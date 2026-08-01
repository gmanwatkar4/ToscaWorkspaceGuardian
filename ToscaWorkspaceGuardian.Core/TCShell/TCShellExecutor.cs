using ToscaWorkspaceGuardian.Common.Interfaces;
using ToscaWorkspaceGuardian.Core.Models;

namespace ToscaWorkspaceGuardian.Core.TCShell;

public class TCShellExecutor
{
    private readonly IToscaInstallationService _installationService;
    private readonly IProcessRunner _processRunner;

    public TCShellExecutor(
        IToscaInstallationService installationService,
        IProcessRunner processRunner)
    {
        _installationService = installationService;
        _processRunner = processRunner;
    }

    public async Task<TCShellResponse> ExecuteAsync(
        string scriptFile,
        WorkspaceRequest request)
    {
        var installation = _installationService.GetInstallation();

        if (!installation.IsInstalled)
        {
            return new TCShellResponse
            {
                Success = false,
                ExitCode = -1,
                Error = "TCShell installation not found."
            };
        }

        string arguments =
            $"-workspace \"{request.WorkspacePath}\" " +
            $"-login {request.Username} {request.Password} " +
            $"\"{scriptFile}\"";

        var result = await _processRunner.ExecuteAsync(
            installation.TCShellPath,
            arguments);

        return new TCShellResponse
        {
            Success = result.ExitCode == 0,
            ExitCode = result.ExitCode,
            Output = result.StandardOutput,
            Error = result.StandardError
        };
    }
}