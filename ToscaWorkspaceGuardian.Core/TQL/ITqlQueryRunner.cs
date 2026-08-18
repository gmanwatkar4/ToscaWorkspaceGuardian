namespace ToscaWorkspaceGuardian.Core.TQL;

using ToscaWorkspaceGuardian.Core.Models;

public interface ITqlQueryRunner
{
    Task<TqlQueryResult> RunAsync(
        string query,
        WorkspaceRequest request,
        CancellationToken cancellationToken = default);
}
