// <copyright file="ScriptService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Script;

using ToscaWorkspaceGuardian.Core.Interfaces;
using ToscaWorkspaceGuardian.Core.Models;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;

public class ScriptService : IScriptService
{
    private readonly ScriptComposer composer;
    private readonly RepositoryScanScriptBuilder repositoryBuilder;
    private readonly PrintObjectScriptBuilder printBuilder;
    private readonly string tempDirectory;
    private readonly System.Collections.Concurrent.ConcurrentDictionary<string, string> scriptCache = new();
    private readonly int _maxCacheFiles = 100;

    public ScriptService(
        ScriptComposer composer,
        RepositoryScanScriptBuilder repositoryBuilder,
        PrintObjectScriptBuilder printBuilder)
    {
        this.composer = composer;
        this.repositoryBuilder = repositoryBuilder;
        this.printBuilder = printBuilder;
        this.tempDirectory = Path.Combine(Path.GetTempPath(), "ToscaWorkspaceGuardian", "Scripts");
        Directory.CreateDirectory(this.tempDirectory);
    }

    public async Task<string> GenerateScriptAsync(
        ScriptRequest request,
        CancellationToken cancellationToken = default)
    {
        // Reuse a scripts directory and cache script files by content hash to avoid rewriting identical files
        if (request.ScriptType == ScriptType.RepositoryScan)
        {
            string repositoryScript = this.repositoryBuilder.Build(request.Queries);
            string repoHash = ComputeHash(repositoryScript);

            if (this.scriptCache.TryGetValue(repoHash, out var existingRepoPath) && File.Exists(existingRepoPath))
            {
                return existingRepoPath;
            }

            string repositoryFile = Path.Combine(this.tempDirectory, $"RepositoryScan_{repoHash}.tcs");
            await File.WriteAllTextAsync(repositoryFile, repositoryScript, cancellationToken);

            string printFile = Path.Combine(this.tempDirectory, "PrintObject.tcs");
            if (!File.Exists(printFile))
            {
                await File.WriteAllTextAsync(printFile, this.printBuilder.Build(), cancellationToken);
            }

            this.scriptCache[repoHash] = repositoryFile;
            EnforceCacheLimit();
            return repositoryFile;
        }

        string script = this.composer.Compose(request);
        string scriptHash = ComputeHash(script);

        if (this.scriptCache.TryGetValue(scriptHash, out var existing) && File.Exists(existing))
        {
            return existing;
        }

        string workspaceFile = Path.Combine(this.tempDirectory, $"WorkspaceAnalysis_{scriptHash}.tcs");
        await File.WriteAllTextAsync(workspaceFile, script, cancellationToken);
        this.scriptCache[scriptHash] = workspaceFile;
        EnforceCacheLimit();
        return workspaceFile;
    }

    private static string ComputeHash(string content)
    {
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(content);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private void EnforceCacheLimit()
    {
        try
        {
            if (this.scriptCache.Count <= this._maxCacheFiles) return;

            var files = Directory.GetFiles(this.tempDirectory, "*.tcs")
                .Select(f => new FileInfo(f))
                .OrderBy(fi => fi.LastWriteTimeUtc)
                .ToList();

            int toRemove = Math.Max(0, files.Count - this._maxCacheFiles);
            for (int i = 0; i < toRemove; i++)
            {
                try
                {
                    File.Delete(files[i].FullName);
                }
                catch { }
            }
        }
        catch { }
    }
}
