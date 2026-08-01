using ToscaWorkspaceGuardian.Core.Models;

namespace ToscaWorkspaceGuardian.Core.Interfaces;

public interface ITCShellService
{
    Task<TCShellResponse> ExecuteScriptAsync(
        string scriptFile,
        WorkspaceRequest request,
        CancellationToken cancellationToken = default);
}