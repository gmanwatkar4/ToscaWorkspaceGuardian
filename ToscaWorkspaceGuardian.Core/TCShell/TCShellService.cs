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
        var execution =
            await _executor.ExecuteAsync(
                scriptFile,
                request);

        return new TCShellResponse
        {
            Success = execution.Success,
            ExitCode = execution.ExitCode,
            Output = execution.StandardOutput,
            Error = execution.StandardError
        };
    }
}