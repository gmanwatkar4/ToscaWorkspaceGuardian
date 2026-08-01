namespace ToscaWorkspaceGuardian.Core.Models;

public class WorkspaceInfo
{
    public string WorkspacePath { get; set; } = string.Empty;

    public string RepositoryType { get; set; } = string.Empty;

    public bool IsManagedRepository { get; set; }

    public bool RequiresUserPassword { get; set; }

    public bool RequiresClientSecret { get; set; }

    public string ConnectionData { get; set; } = string.Empty;

    public string ProjectId { get; set; } = string.Empty;

    public string DefaultUser { get; set; } = string.Empty;
}