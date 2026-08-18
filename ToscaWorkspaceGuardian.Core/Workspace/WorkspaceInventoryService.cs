namespace ToscaWorkspaceGuardian.Core.Workspace;

using System.Text.Json;
using ToscaWorkspaceGuardian.Core.Business;
using ToscaWorkspaceGuardian.Core.Interfaces;
using ToscaWorkspaceGuardian.Core.Models;
using ToscaWorkspaceGuardian.Core.TQL;

/// <summary>
/// Maintains a durable JSON inventory. Later reads inspect known folders for new children while
/// deliberately skipping the detailed print of already-known leaf nodes.
/// </summary>
public sealed class WorkspaceInventoryService : IWorkspaceInventoryService
{
    private static readonly string[] UniqueIdPropertyNames = new[] { "UniqueId", "UniqueID", "TCId", "Id", "ObjectId" };
    private static readonly (string Name, string Query)[] SupplementalArtifactQueries =
    {
        ("module attributes", "=>SUBPARTS:XModuleAttribute"),
        ("test steps", "=>SUBPARTS:TestStep"),
        ("test step values", "=>SUBPARTS:TestStepValue"),
        ("execution entries", "=>SUBPARTS:ExecutionEntry"),
        ("requirements", "=>SUBPARTS:Requirement"),
    };
    private readonly IWorkspaceCrawler crawler;
    private readonly IWorkspaceReadActivity activity;
    private readonly ITqlQueryRunner tqlQueryRunner;

    public WorkspaceInventoryService(
        IWorkspaceCrawler crawler,
        IWorkspaceReadActivity activity,
        ITqlQueryRunner tqlQueryRunner)
    {
        this.crawler = crawler;
        this.activity = activity;
        this.tqlQueryRunner = tqlQueryRunner;
    }

    public async Task<WorkspaceInventoryResult> AnalyzeAsync(
        WorkspaceRequest request,
        CancellationToken cancellationToken = default)
    {
        var outputFolder = string.IsNullOrWhiteSpace(request.OutputFolder)
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ToscaUpgradeGuide")
            : request.OutputFolder;
        var filePath = Path.Combine(outputFolder, "WorkspaceInventory.json");
        var existingInventory = await LoadExistingInventoryAsync(filePath, request.WorkspacePath, cancellationToken);
        var knownExpandableNodePaths = existingInventory?.Nodes
            .Where(node => node.Collections.ContainsKey("Items") || IsKnownFolderType(node.ObjectType))
            .Select(node => node.NodePath)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var isIncremental = knownExpandableNodePaths is { Count: > 0 };

        this.activity.Report(isIncremental
            ? $"Loaded {knownExpandableNodePaths!.Count:N0} previously inventoried expandable node(s). Looking for additions only."
            : "No matching prior inventory found. Performing the initial full read.");

        var snapshot = await this.crawler.CrawlAsync(request, knownExpandableNodePaths, cancellationToken);
        var readNodes = snapshot.Objects
            .Where(item => !string.IsNullOrWhiteSpace(item.NodePath))
            .GroupBy(item => item.NodePath, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.Last())
            .Select(ToInventoryNode)
            .ToList();

        var supplementalCount = 0;
        foreach (var (name, query) in SupplementalArtifactQueries)
        {
            this.activity.Report($"Supplemental batch: submitting {name} query.");
            var queryResult = await this.tqlQueryRunner.RunAsync(query, request, cancellationToken);
            if (!queryResult.Success)
            {
                this.activity.Report($"Supplemental batch failed: {name}. Inventory JSON was not updated.", WorkspaceReadActivityLevel.Error);
                throw new InvalidOperationException($"TQL query for {name} failed. {queryResult.Message}");
            }

            var batchNodes = queryResult.Document.Objects
                .Select(ToInventoryNode)
                .Where(node => !string.IsNullOrWhiteSpace(node.UniqueId) || !string.IsNullOrWhiteSpace(node.NodePath))
                .ToList();
            readNodes.AddRange(batchNodes);
            supplementalCount += batchNodes.Count;
            this.activity.Report($"Supplemental batch complete: {name}; {batchNodes.Count:N0} artifact(s) parsed.");
        }

        this.activity.Report($"Supplemental batches complete: {supplementalCount:N0} nested artifact(s) ready to merge.");

        if (readNodes.Count == 0)
        {
            this.activity.Report("No readable workspace nodes were returned. Inventory JSON was not written.", WorkspaceReadActivityLevel.Error);
            throw new InvalidOperationException(
                "TCShell returned no readable workspace nodes. No inventory JSON was written; verify workspace access and TCShell output.");
        }

        var nodesByKey = existingInventory?.Nodes.ToDictionary(
            GetInventoryKey,
            StringComparer.OrdinalIgnoreCase)
            ?? new Dictionary<string, WorkspaceInventoryNode>(StringComparer.OrdinalIgnoreCase);
        var newNodeCount = readNodes
            .Select(GetInventoryKey)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Count(key => !nodesByKey.ContainsKey(key));
        foreach (var node in readNodes)
        {
            nodesByKey[GetInventoryKey(node)] = node;
        }

        this.activity.Report("Inventory merge complete; writing the updated JSON inventory.");

        var nodes = nodesByKey.Values
            .OrderBy(node => node.NodePath, StringComparer.OrdinalIgnoreCase)
            .ToList();
        var inventory = new WorkspaceInventorySnapshot
        {
            GeneratedAtUtc = DateTime.UtcNow,
            WorkspacePath = request.WorkspacePath,
            SourceVersion = request.SourceVersion,
            TargetVersion = request.TargetVersion,
            ObjectCount = nodes.Count,
            AnalysisMode = isIncremental ? "Incremental" : "Full",
            NewNodeCount = newNodeCount,
            Nodes = nodes,
        };

        await WriteAtomicallyAsync(inventory, filePath, cancellationToken);
        this.activity.Report($"Inventory JSON written: {filePath}. {newNodeCount:N0} new node(s); {nodes.Count:N0} total node(s).");

        return new WorkspaceInventoryResult
        {
            InventoryFilePath = filePath,
            ObjectCount = nodes.Count,
            NewNodeCount = newNodeCount,
            IsIncremental = isIncremental,
        };
    }

    private static WorkspaceInventoryNode ToInventoryNode(RepositoryObject item) => new()
    {
        NodePath = item.NodePath,
        ParentPath = item.ParentPath,
        UniqueId = FindUniqueId(item),
        ObjectType = item.ObjectType,
        Name = item.Name,
        Properties = new Dictionary<string, string>(item.Properties, StringComparer.OrdinalIgnoreCase),
        Collections = item.Collections.ToDictionary(
            pair => pair.Key,
            pair => new List<string>(pair.Value),
            StringComparer.OrdinalIgnoreCase),
    };

    private static WorkspaceInventoryNode ToInventoryNode(OutputObject item)
    {
        var properties = item.Properties.ToDictionary(
            property => property.Name,
            property => property.Value,
            StringComparer.OrdinalIgnoreCase);
        var nodePath = properties.TryGetValue("NodePath", out var resolvedNodePath)
            ? resolvedNodePath
            : string.Empty;
        var uniqueId = FindUniqueId(properties, nodePath);
        return new WorkspaceInventoryNode
        {
            NodePath = nodePath,
            ParentPath = FindParentPath(nodePath),
            UniqueId = uniqueId,
            ObjectType = item.ObjectType,
            Name = item.Name,
            Properties = properties,
            Collections = item.Collections.ToDictionary(
                pair => pair.Key,
                pair => new List<string>(pair.Value),
                StringComparer.OrdinalIgnoreCase),
        };
    }

    private static string FindUniqueId(RepositoryObject item)
        => FindUniqueId(item.Properties, item.NodePath);

    private static string FindUniqueId(IReadOnlyDictionary<string, string> properties, string nodePath)
    {
        foreach (var propertyName in UniqueIdPropertyNames)
        {
            var match = properties.FirstOrDefault(pair =>
                string.Equals(pair.Key, propertyName, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(match.Value))
            {
                return match.Value;
            }
        }

        return nodePath;
    }

    private static string FindParentPath(string nodePath)
    {
        var separator = nodePath.LastIndexOf('/');
        return separator > 0 ? nodePath[..separator] : string.Empty;
    }

    private static string GetInventoryKey(WorkspaceInventoryNode node) =>
        !string.IsNullOrWhiteSpace(node.NodePath)
            ? $"Path:{node.NodePath}"
            : $"UniqueId:{node.UniqueId}";

    private static bool IsKnownFolderType(string objectType) =>
        objectType is "TCProject" or "TCFolder" or "OwnedFolder" or "ExecutionEntryFolder";

    private static async Task<WorkspaceInventorySnapshot?> LoadExistingInventoryAsync(
        string filePath,
        string workspacePath,
        CancellationToken cancellationToken)
    {
        if (!File.Exists(filePath))
        {
            return null;
        }

        try
        {
            var json = await File.ReadAllTextAsync(filePath, cancellationToken);
            var inventory = JsonSerializer.Deserialize<WorkspaceInventorySnapshot>(json);
            return inventory is not null
                && string.Equals(inventory.SchemaVersion, "1.4", StringComparison.Ordinal)
                && string.Equals(
                    Path.GetFullPath(inventory.WorkspacePath),
                    Path.GetFullPath(workspacePath),
                    StringComparison.OrdinalIgnoreCase)
                ? inventory
                : null;
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static async Task WriteAtomicallyAsync(
        WorkspaceInventorySnapshot inventory,
        string destinationPath,
        CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(destinationPath)!);
        var temporaryPath = destinationPath + ".tmp";
        var json = JsonSerializer.Serialize(inventory, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(temporaryPath, json, cancellationToken);
        File.Move(temporaryPath, destinationPath, true);
    }
}

/// <summary>Durable JSON schema for the workspace inventory.</summary>
public sealed class WorkspaceInventorySnapshot
{
    public string SchemaVersion { get; init; } = "1.4";

    public DateTime GeneratedAtUtc { get; init; }

    public string WorkspacePath { get; init; } = string.Empty;

    public string SourceVersion { get; init; } = string.Empty;

    public string TargetVersion { get; init; } = string.Empty;

    public int ObjectCount { get; init; }

    public string AnalysisMode { get; init; } = "Full";

    public int NewNodeCount { get; init; }

    public List<WorkspaceInventoryNode> Nodes { get; init; } = new();
}

/// <summary>One Tosca repository node, including its complete printed property set.</summary>
public sealed class WorkspaceInventoryNode
{
    public string NodePath { get; init; } = string.Empty;

    public string ParentPath { get; init; } = string.Empty;

    public string UniqueId { get; init; } = string.Empty;

    public string ObjectType { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public Dictionary<string, string> Properties { get; init; } = new();

    public Dictionary<string, List<string>> Collections { get; init; } = new();
}
