using ToscaWorkspaceGuardian.Core.Business;
using ToscaWorkspaceGuardian.Core.Interfaces;

namespace ToscaWorkspaceGuardian.Core.TCShell;

public class NodePathResolver : INodePathResolver
{
    public string GetNodePath(RepositoryObject obj)
    {
        return obj.NodePath;
    }
}