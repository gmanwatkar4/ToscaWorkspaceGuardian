using ToscaWorkspaceGuardian.Core.Models;

namespace ToscaWorkspaceGuardian.Core.Interfaces;

public interface IWorkspaceReader
{
    Task<WorkspaceSummary> ReadAsync(
        WorkspaceRequest request,
        CancellationToken cancellationToken = default);
}