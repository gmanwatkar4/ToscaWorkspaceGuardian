// <copyright file="WorkspaceCrawler.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Traversal;

using ToscaWorkspaceGuardian.Core.Business;
using ToscaWorkspaceGuardian.Core.Interfaces;
using ToscaWorkspaceGuardian.Core.Models;
using ToscaWorkspaceGuardian.Core.Script;
using ToscaWorkspaceGuardian.Core.TCShell;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;

public class WorkspaceCrawler : IWorkspaceCrawler
{
    private readonly BatchScriptBuilder scriptBuilder;
    private readonly ITCShellService tcShellService;
    private readonly OutputParser parser;
    private readonly ISnapshotBuilder snapshotBuilder;
    private readonly int maxDegreeOfParallelism = 4;
    private readonly object snapshotLock = new();
    private readonly ToscaWorkspaceGuardian.Core.Diagnostics.TelemetryCollector? telemetry;
    private readonly Microsoft.Extensions.Logging.ILogger<WorkspaceCrawler>? logger;
    private readonly string cacheDirectory;

    public WorkspaceCrawler(
        BatchScriptBuilder scriptBuilder,
        ITCShellService tcShellService,
        OutputParser parser,
        ISnapshotBuilder snapshotBuilder,
        ToscaWorkspaceGuardian.Core.Diagnostics.TelemetryCollector? telemetry = null,
        Microsoft.Extensions.Logging.ILogger<WorkspaceCrawler>? logger = null)
    {
        this.scriptBuilder = scriptBuilder;
        this.tcShellService = tcShellService;
        this.parser = parser;
        this.snapshotBuilder = snapshotBuilder;
        this.telemetry = telemetry;
        this.logger = logger;
        this.cacheDirectory = Path.Combine(Path.GetTempPath(), "ToscaWorkspaceGuardian", "Cache");
        Directory.CreateDirectory(this.cacheDirectory);
    }

    public async Task<WorkspaceSnapshot> CrawlAsync(
    WorkspaceRequest request,
    CancellationToken cancellationToken = default)
    {
        //------------------------------------------
        // Repository Queue
        //------------------------------------------
        var queue = new Queue<string>();

        var visited = new HashSet<string>(
        StringComparer.OrdinalIgnoreCase);

        EnqueueIfNew("/Modules");
        EnqueueIfNew("/TestCases");
        EnqueueIfNew("/Execution");
        EnqueueIfNew("/Requirements");

        var snapshot = new WorkspaceSnapshot();

        const int BatchSize = 50;

        //------------------------------------------
        // Crawl Until Queue Empty (bounded concurrency)
        //------------------------------------------
        var crawlSw = System.Diagnostics.Stopwatch.StartNew();

        var queueLock = new object();
        var visitedLock = new object();
        var runningTasks = new List<Task>();

        while (queue.Count > 0 || runningTasks.Count > 0)
        {
            // Start new tasks up to maxDegreeOfParallelism
            while (runningTasks.Count < maxDegreeOfParallelism)
            {
                List<string> paths = new List<string>();
                lock (queueLock)
                {
                    while (queue.Count > 0 && paths.Count < BatchSize)
                    {
                        paths.Add(queue.Dequeue());
                    }
                }

                if (paths.Count == 0) break;

                var task = Task.Run(async () =>
                {
                    string? responseOutput = null;
                    string? responseError = null;
                    int responseExitCode = 0;
                    object? document = null;

                    try
                    {
                        System.Diagnostics.Debug.WriteLine("EXECUTING BATCH");
                        foreach (var p in paths.Take(10))
                        {
                            System.Diagnostics.Debug.WriteLine(p);
                        }

                        var script = this.scriptBuilder.Build(paths);
                        var scriptFile = Path.Combine(Path.GetTempPath(), "ToscaWorkspaceGuardian", "Crawler.tcs");
                        Directory.CreateDirectory(Path.GetDirectoryName(scriptFile)!);
                        await File.WriteAllTextAsync(scriptFile, script, cancellationToken);

                        var batchKey = ComputeHash(string.Join("|", paths));
                        var cacheFile = Path.Combine(this.cacheDirectory, $"batch_{batchKey}.json");

                        string? cachedOutput = null;

                        if (File.Exists(cacheFile))
                        {
                            try
                            {
                                var json = await File.ReadAllTextAsync(cacheFile, cancellationToken);
                                var dict = System.Text.Json.JsonSerializer.Deserialize<System.Collections.Generic.Dictionary<string, string>>(json);
                                cachedOutput = dict != null && dict.TryGetValue("Output", out var o) ? o : string.Empty;
                                responseOutput = cachedOutput;
                                responseError = dict != null && dict.TryGetValue("Error", out var e) ? e : null;
                                responseExitCode = dict != null && dict.TryGetValue("ExitCode", out var c) && int.TryParse(c, out var ci) ? ci : 0;
                                this.logger?.LogDebug("Loaded cached batch result {CacheFile}", cacheFile);
                            }
                            catch (Exception ex)
                            {
                                this.logger?.LogWarning(ex, "Failed to read cache file {CacheFile}, will execute TCShell", cacheFile);
                            }
                        }

                        if (cachedOutput != null)
                        {
                            document = this.parser.Parse(cachedOutput);
                        }
                        else
                        {
                            var response = await this.tcShellService.ExecuteScriptAsync(scriptFile, request, cancellationToken);
                            responseOutput = response.Output ?? string.Empty;
                            responseError = response.Error;
                            responseExitCode = response.ExitCode;
                            document = this.parser.Parse(responseOutput);
                            try
                            {
                                var ser = System.Text.Json.JsonSerializer.Serialize(new { Output = responseOutput, Error = responseError, ExitCode = responseExitCode });
                                await File.WriteAllTextAsync(cacheFile, ser, cancellationToken);
                            }
                            catch { }
                        }

                        var parsedDocument = (document is null) ? this.parser.Parse(responseOutput ?? string.Empty) : (dynamic)document;

                        // Merge snapshot
                        lock (snapshotLock)
                        {
                            this.snapshotBuilder.AddDocument(snapshot, parsedDocument);
                        }

                        // Discover children and enqueue
                        foreach (var obj in parsedDocument.Objects)
                        {
                            if (!IsContainer(obj.ObjectType)) continue;
                            string? nodePath = obj.GetProperty("NodePath");
                            if (string.IsNullOrWhiteSpace(nodePath)) continue;
                            foreach (var child in obj.GetCollection("Items"))
                            {
                                var childPath = $"{nodePath}/{child}";
                                lock (visitedLock)
                                {
                                    if (visited.Add(childPath))
                                    {
                                        lock (queueLock)
                                        {
                                            queue.Enqueue(childPath);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        this.logger?.LogWarning(ex, "Batch processing failed");
                    }
                }, cancellationToken);

                runningTasks.Add(task);
            }

            if (runningTasks.Count == 0)
            {
                // Nothing is running and nothing was started (queue empty)
                break;
            }

            // Wait for any task to complete
            var completed = await Task.WhenAny(runningTasks);
            // Remove completed tasks
            runningTasks.RemoveAll(t => t.IsCompleted);
        }

        void EnqueueIfNew(string path)
        {
            if (visited.Add(path))
            {
                queue.Enqueue(path);
            }
        }

        static bool IsContainer(string objectType)
        {
            return objectType switch
            {
                "TCProject" => true,
                "TCFolder" => true,
                "OwnedFolder" => true,
                "ExecutionEntryFolder" => true,
                _ => false,
            };
        }

        crawlSw.Stop();

        try
        {
            var summary = this.telemetry?.GetSummary();
            if (!string.IsNullOrWhiteSpace(summary))
            {
                this.logger?.LogInformation(summary);
            }
        }
        catch { }

        return snapshot;
    }

    private static string ComputeHash(string content)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(content);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}

