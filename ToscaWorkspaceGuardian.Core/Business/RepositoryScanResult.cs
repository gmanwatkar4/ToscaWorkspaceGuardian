namespace ToscaWorkspaceGuardian.Core.Business;

public class RepositoryScanResult
{
    public List<RepositoryNode> Nodes { get; } = new();

    public ParsedWorkspace Workspace { get; } = new();
}