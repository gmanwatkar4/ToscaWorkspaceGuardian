using ToscaWorkspaceGuardian.Core.Business;

namespace ToscaWorkspaceGuardian.Core.Interfaces;

public interface INodePathResolver
{
    string GetNodePath(RepositoryObject obj);
}