namespace ToscaWorkspaceGuardian.UI.ViewModels;

/// <summary>One safe, user-visible entry in the workspace read activity monitor.</summary>
public sealed class WorkspaceActivityEntry
{
    public string Timestamp { get; init; } = string.Empty;

    public string Level { get; init; } = string.Empty;

    public string Message { get; init; } = string.Empty;
}
