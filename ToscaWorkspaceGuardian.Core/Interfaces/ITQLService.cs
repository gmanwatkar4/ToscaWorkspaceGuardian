using ToscaWorkspaceGuardian.Core.Models;

namespace ToscaWorkspaceGuardian.Core.Interfaces;

public interface ITQLService
{
    Task<TQLResult> ExecuteAsync(
        string query,
        WorkspaceRequest request,
        CancellationToken cancellationToken = default);
}