namespace ToscaWorkspaceGuardian.Core.TCShell;

using ToscaWorkspaceGuardian.Common.Interfaces;

/// <summary>
/// Validates that the installed TC-Shell can start without opening a workspace.
/// </summary>
public sealed class TCShellPreflightService : ITCShellPreflightService
{
    private readonly IToscaInstallationService installationService;
    private readonly IProcessRunner processRunner;

    public TCShellPreflightService(
        IToscaInstallationService installationService,
        IProcessRunner processRunner)
    {
        this.installationService = installationService;
        this.processRunner = processRunner;
    }

    public async Task<TCShellPreflightResult> CheckAsync(CancellationToken cancellationToken = default)
    {
        var installation = this.installationService.GetInstallation();
        if (!installation.IsInstalled)
        {
            return new TCShellPreflightResult
            {
                IsInstalled = false,
                Message = "TCShell.exe was not found in the Tosca installation.",
            };
        }

        var execution = await this.processRunner.ExecuteAsync(installation.TCShellPath, "-help", cancellationToken);
        var combinedOutput = $"{execution.StandardOutput}\n{execution.StandardError}";
        var licenseDetected = combinedOutput.Contains("License found", StringComparison.OrdinalIgnoreCase);

        if (execution.ExitCode == 0 && licenseDetected)
        {
            return new TCShellPreflightResult
            {
                IsReady = true,
                IsInstalled = true,
                LicenseDetected = true,
                TCShellPath = installation.TCShellPath,
                Message = "TCShell started successfully and a Tosca license was detected.",
            };
        }

        var permissionIssue = combinedOutput.Contains("Settings.xml", StringComparison.OrdinalIgnoreCase)
            || combinedOutput.Contains("Access", StringComparison.OrdinalIgnoreCase);
        return new TCShellPreflightResult
        {
            IsInstalled = true,
            LicenseDetected = licenseDetected,
            TCShellPath = installation.TCShellPath,
            Message = permissionIssue
                ? "TCShell could not read its user settings. Run Workspace Guardian with the same Windows permissions as Tosca Commander."
                : "TCShell did not start cleanly. Review Tosca installation, user settings, and license configuration.",
        };
    }
}
