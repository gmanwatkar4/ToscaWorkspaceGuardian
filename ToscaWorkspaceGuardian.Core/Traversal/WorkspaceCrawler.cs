namespace ToscaWorkspaceGuardian.Core.Traversal;

using ToscaWorkspaceGuardian.Core.Business;
using ToscaWorkspaceGuardian.Core.Interfaces;
using ToscaWorkspaceGuardian.Core.Models;
using ToscaWorkspaceGuardian.Core.Script;
using ToscaWorkspaceGuardian.Core.TCShell;
using ToscaWorkspaceGuardian.Core.Workspace;

/// <summary>
/// Reads a workspace tree through TCShell without making repository changes.
/// Each run obtains fresh output so an inventory never contains results from a different workspace.
/// </summary>
public sealed class WorkspaceCrawler : IWorkspaceCrawler
{
    private const int NodeBatchSize = 50;
    private static readonly TimeSpan BatchTimeout = TimeSpan.FromMinutes(2);
    private readonly BatchScriptBuilder scriptBuilder;
    private readonly ITCShellService tcShellService;
    private readonly OutputParser parser;
    private readonly ISnapshotBuilder snapshotBuilder;
    private readonly IWorkspaceReadActivity activity;

    public WorkspaceCrawler(
        BatchScriptBuilder scriptBuilder,
        ITCShellService tcShellService,
        OutputParser parser,
        ISnapshotBuilder snapshotBuilder,
        IWorkspaceReadActivity activity)
    {
        this.scriptBuilder = scriptBuilder;
        this.tcShellService = tcShellService;
        this.parser = parser;
        this.snapshotBuilder = snapshotBuilder;
        this.activity = activity;
    }

    public async Task<WorkspaceSnapshot> CrawlAsync(
        WorkspaceRequest request,
        IReadOnlySet<string>? knownExpandableNodePaths = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.WorkspacePath);

        var queue = new Queue<string>(new[] { "/Modules", "/TestCases", "/Execution", "/Requirements" });
        var scheduled = new HashSet<string>(queue, StringComparer.OrdinalIgnoreCase);
        var snapshot = new WorkspaceSnapshot();
        var incremental = knownExpandableNodePaths is { Count: > 0 };
        this.activity.Report(incremental
            ? "Incremental workspace read started. Existing expandable nodes will be probed; known leaves will be skipped."
            : "Full workspace read started. Every reachable Items hierarchy node will be read.");

        while (queue.Count > 0)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var batch = new List<string>(NodeBatchSize);
            while (queue.Count > 0 && batch.Count < NodeBatchSize)
            {
                batch.Add(queue.Dequeue());
            }

            this.activity.Report($"Reading batch of {batch.Count:N0} node(s): {batch[0]}{(batch.Count > 1 ? " …" : string.Empty)}");
            var scriptPath = await this.WriteReadOnlyScriptAsync(batch, cancellationToken);
            using var batchCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            batchCancellation.CancelAfter(BatchTimeout);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var response = await this.tcShellService.ExecuteScriptAsync(scriptPath, request, batchCancellation.Token);
            stopwatch.Stop();

            if (batchCancellation.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
            {
                this.activity.Report(
                    $"TCShell timed out after {BatchTimeout.TotalMinutes:N0} minute(s) while reading a batch beginning at: {batch[0]}",
                    WorkspaceReadActivityLevel.Error);
                throw new TimeoutException($"TCShell did not complete the batch beginning at '{batch[0]}' within {BatchTimeout.TotalMinutes:N0} minutes.");
            }

            cancellationToken.ThrowIfCancellationRequested();
            if (!response.Success)
            {
                this.activity.Report($"TCShell failed while reading a batch beginning at: {batch[0]}", WorkspaceReadActivityLevel.Error);
                throw new InvalidOperationException(
                    $"TCShell could not read the batch beginning at '{batch[0]}'. {response.Error}".Trim());
            }

            var document = this.parser.Parse(response.Output);
            this.activity.Report($"TCShell completed for batch in {stopwatch.Elapsed.TotalSeconds:N1}s; parsed {document.Objects.Count:N0} object(s).");
            if (document.Objects.Count == 0)
            {
                continue;
            }

            this.snapshotBuilder.AddDocument(snapshot, document);

            var queuedChildren = 0;
            foreach (var item in document.Objects)
            {
                var parentPath = item.GetProperty("NodePath");
                if (string.IsNullOrWhiteSpace(parentPath))
                {
                    continue;
                }

                foreach (var childName in item.GetCollection("Items"))
                {
                    var childPath = $"{parentPath.TrimEnd('/')}/{childName}";
                    if (incremental
                        && !knownExpandableNodePaths!.Contains(childPath))
                    {
                        continue;
                    }

                    if (scheduled.Add(childPath))
                    {
                        queue.Enqueue(childPath);
                        queuedChildren++;
                    }
                }
            }

            if (queuedChildren > 0)
            {
                this.activity.Report($"Queued {queuedChildren:N0} new child node(s) discovered from this batch.");
            }
        }

        this.activity.Report($"Workspace read completed. {snapshot.ByNodePath.Count:N0} unique node path(s) were captured.");
        return snapshot;
    }

    private async Task<string> WriteReadOnlyScriptAsync(IReadOnlyCollection<string> nodePaths, CancellationToken cancellationToken)
    {
        var runFolder = Path.Combine(Path.GetTempPath(), "ToscaUpgradeGuide", "WorkspaceRead", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(runFolder);
        var scriptPath = Path.Combine(runFolder, "ReadBatch.tcs");
        await File.WriteAllTextAsync(scriptPath, this.scriptBuilder.Build(nodePaths), cancellationToken);
        return scriptPath;
    }
}
