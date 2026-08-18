namespace ToscaWorkspaceGuardian.Core.Workspace;

/// <summary>Summary of one completed workspace inventory run.</summary>
public sealed class WorkspaceInventoryResult
{
    public string InventoryFilePath { get; init; } = string.Empty;

    public int ObjectCount { get; init; }

    public int NewNodeCount { get; init; }

    public bool IsIncremental { get; init; }
}
