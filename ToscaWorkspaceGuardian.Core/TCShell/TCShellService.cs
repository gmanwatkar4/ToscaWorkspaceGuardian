using ToscaWorkspaceGuardian.Core.Interfaces;
using ToscaWorkspaceGuardian.Core.Models;

namespace ToscaWorkspaceGuardian.Core.TCShell;

public class TCShellService : ITCShellService
{
    private readonly TCShellExecutor _executor;

    public TCShellService(
        TCShellExecutor executor)
    {
        _executor = executor;
    }

    public async Task<TCShellResponse> ExecuteScriptAsync(
        string scriptFile,
        WorkspaceRequest request,
        CancellationToken cancellationToken = default)
    {
        return await _executor.ExecuteAsync(
            scriptFile,
            request);
    }
}