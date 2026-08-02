using ToscaWorkspaceGuardian.Core.Business;
using ToscaWorkspaceGuardian.Core.Models;

namespace ToscaWorkspaceGuardian.Core.Interfaces;

public interface IRepositoryTreeWalker
{
    Task<ParsedWorkspace> ScanAsync(
        WorkspaceRequest request,
        CancellationToken cancellationToken = default);
}