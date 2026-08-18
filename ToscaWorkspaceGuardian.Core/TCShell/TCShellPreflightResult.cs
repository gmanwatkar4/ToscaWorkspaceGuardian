namespace ToscaWorkspaceGuardian.Core.TCShell;

/// <summary>
/// Result of a safe TCShell availability and license preflight.
/// </summary>
public sealed class TCShellPreflightResult
{
    public bool IsReady { get; init; }

    public bool IsInstalled { get; init; }

    public bool LicenseDetected { get; init; }

    public string TCShellPath { get; init; } = string.Empty;

    public string Message { get; init; } = string.Empty;
}
