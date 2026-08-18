namespace ToscaWorkspaceGuardian.Core.Upgrade;

public sealed class UpgradeRiskScanResult
{
    public IReadOnlyList<UpgradeRiskFinding> Findings { get; init; } = Array.Empty<UpgradeRiskFinding>();

    public IReadOnlyList<string> Diagnostics { get; init; } = Array.Empty<string>();
}
