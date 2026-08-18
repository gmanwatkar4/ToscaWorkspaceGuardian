namespace ToscaWorkspaceGuardian.Core.AI;

using ToscaWorkspaceGuardian.Core.Models;

public sealed class WorkspaceNavigationResponse
{
    public bool Success { get; init; }

    public string GeneratedTql { get; init; } = string.Empty;

    public string Answer { get; init; } = string.Empty;

    public OutputDocument Results { get; init; } = new();
}
