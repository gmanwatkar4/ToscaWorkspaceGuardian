namespace ToscaWorkspaceGuardian.Core.Models;

public class WorkspaceSummary
{
    public WorkspaceInfo WorkspaceInfo { get; set; } = new();

    public WorkspaceMetadata Metadata { get; set; } = new();

    public WorkspaceStatistics Statistics { get; set; } = new();
}