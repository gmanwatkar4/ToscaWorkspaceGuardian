using ToscaWorkspaceGuardian.Core.Business;
using ToscaWorkspaceGuardian.Core.Interfaces;
using ToscaWorkspaceGuardian.Core.Models;
using ToscaWorkspaceGuardian.Core.Script;
using ToscaWorkspaceGuardian.Core.TCShell;

namespace ToscaWorkspaceGuardian.Core.Traversal;

public class WorkspaceCrawler : IWorkspaceCrawler
{
    private readonly BatchScriptBuilder _scriptBuilder;
    private readonly ITCShellService _tcShellService;
    private readonly OutputParser _parser;
    private readonly ISnapshotBuilder _snapshotBuilder;

    public WorkspaceCrawler(
        BatchScriptBuilder scriptBuilder,
        ITCShellService tcShellService,
        OutputParser parser,
        ISnapshotBuilder snapshotBuilder)
    {
        _scriptBuilder = scriptBuilder;
        _tcShellService = tcShellService;
        _parser = parser;
        _snapshotBuilder = snapshotBuilder;
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
        // Crawl Until Queue Empty
        //------------------------------------------

        while (queue.Count > 0)
        {
            //--------------------------------------
            // Build Current Batch
            //--------------------------------------

            var paths = new List<string>();

            System.Diagnostics.Debug.WriteLine("================================");
            System.Diagnostics.Debug.WriteLine("CURRENT BATCH");

            foreach (var p in queue.Take(10))
            {
                System.Diagnostics.Debug.WriteLine(p);
            }

            System.Diagnostics.Debug.WriteLine("================================");

            while (queue.Count > 0 &&
                   paths.Count < BatchSize)
            {
                paths.Add(queue.Dequeue());
            }

            System.Diagnostics.Debug.WriteLine("EXECUTING");

            foreach (var p in paths.Take(10))
            {
                System.Diagnostics.Debug.WriteLine(p);
            }

            //--------------------------------------
            // Generate Script
            //--------------------------------------

            string script =
                _scriptBuilder.Build(paths);

            //--------------------------------------
            // Save Script
            //--------------------------------------

            string scriptFile =
                Path.Combine(
                    Path.GetTempPath(),
                    "ToscaWorkspaceGuardian",
                    "Crawler.tcs");

            Directory.CreateDirectory(
                Path.GetDirectoryName(scriptFile)!);

            await File.WriteAllTextAsync(
                scriptFile,
                script,
                cancellationToken);

            //--------------------------------------
            // Execute
            //--------------------------------------

            var response =
                await _tcShellService.ExecuteScriptAsync(
                    scriptFile,
                    request,
                    cancellationToken);

            System.Diagnostics.Debug.WriteLine(
                $"ExitCode : {response.ExitCode}");

            System.Diagnostics.Debug.WriteLine(
                $"StdErr Length : {response.Error?.Length ?? 0}");

            //--------------------------------------
            // Diagnostics
            //--------------------------------------

            System.Diagnostics.Debug.WriteLine(
                $"Response Length : {response.Output.Length}");

            //--------------------------------------
            // Parse
            //--------------------------------------

            var document =
                _parser.Parse(response.Output);

            if (document.Objects.Count != paths.Count)
            {
                var debugFolder = Path.Combine(
                    Path.GetTempPath(),
                    "ToscaWorkspaceGuardian",
                    "Debug");

                Directory.CreateDirectory(debugFolder);

                string stamp = DateTime.Now.ToString("yyyyMMdd_HHmmssfff");

                await File.WriteAllTextAsync(
                    Path.Combine(debugFolder, $"Batch_{stamp}.tcs"),
                    script,
                    cancellationToken);

                await File.WriteAllTextAsync(
                    Path.Combine(debugFolder, $"Batch_{stamp}_stdout.txt"),
                    response.Output ?? string.Empty,
                    cancellationToken);

                await File.WriteAllTextAsync(
                    Path.Combine(debugFolder, $"Batch_{stamp}_stderr.txt"),
                    response.Error ?? string.Empty,
                    cancellationToken);
            }

            if (document.Objects.Count != paths.Count)
            {
                var debugFolder = Path.Combine(
                    Path.GetTempPath(),
                    "ToscaWorkspaceGuardian",
                    "Debug");

                Directory.CreateDirectory(debugFolder);

                await File.WriteAllTextAsync(
                    Path.Combine(debugFolder,
                        $"Batch_{DateTime.Now:yyyyMMdd_HHmmss}.tcs"),
                    script,
                    cancellationToken);

                await File.WriteAllTextAsync(
                    Path.Combine(debugFolder,
                        $"Batch_{DateTime.Now:yyyyMMdd_HHmmss}.txt"),
                    response.Output,
                    cancellationToken);
            }

            System.Diagnostics.Debug.WriteLine(
                $"Parser Objects : {document.Objects.Count}");

            //--------------------------------------
            // Merge Snapshot
            //--------------------------------------

            _snapshotBuilder.AddDocument(
                snapshot,
                document);  

            System.Diagnostics.Debug.WriteLine(
                $"Snapshot Objects : {snapshot.Objects.Count}");

            //--------------------------------------
            // Discover Children
            //--------------------------------------

            foreach (var obj in document.Objects)
            {
                //------------------------------------------
                // Only traverse container objects
                //------------------------------------------

                if (!IsContainer(obj.ObjectType))
                    continue;

                string? nodePath = obj.GetProperty("NodePath");

                if (string.IsNullOrWhiteSpace(nodePath))
                    continue;

                foreach (var child in obj.GetCollection("Items"))
                {
                    EnqueueIfNew($"{nodePath}/{child}");
                }
            }

            //--------------------------------------
            // Debug
            //--------------------------------------

            System.Diagnostics.Debug.WriteLine(
                $"Batch Completed : {paths.Count}");

            System.Diagnostics.Debug.WriteLine(
                $"Snapshot Objects : {snapshot.Objects.Count}");

            System.Diagnostics.Debug.WriteLine(
                $"Visited : {visited.Count}");

            System.Diagnostics.Debug.WriteLine(
                $"Remaining Queue : {queue.Count}");
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
                _ => false
            };
        }

        return snapshot;
    }
}