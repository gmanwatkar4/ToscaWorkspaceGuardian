namespace ToscaWorkspaceGuardian.Core.AI;

using ToscaWorkspaceGuardian.Core.Models;

public interface IWorkspaceNavigatorService
{
    Task<WorkspaceNavigationResponse> AskAsync(
        string question,
        WorkspaceRequest request,
        CancellationToken cancellationToken = default);
}
