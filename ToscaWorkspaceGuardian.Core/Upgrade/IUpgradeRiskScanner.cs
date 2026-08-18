namespace ToscaWorkspaceGuardian.Core.Upgrade;

using ToscaWorkspaceGuardian.Core.Models;

public interface IUpgradeRiskScanner
{
    Task<UpgradeRiskScanResult> ScanAsync(
        WorkspaceRequest request,
        CancellationToken cancellationToken = default);
}
