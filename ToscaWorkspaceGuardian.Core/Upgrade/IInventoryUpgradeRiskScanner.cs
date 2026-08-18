namespace ToscaWorkspaceGuardian.Core.Upgrade;

/// <summary>Evaluates documented upgrade rules against a completed, read-only workspace inventory.</summary>
public interface IInventoryUpgradeRiskScanner
{
    Task<UpgradeRiskScanResult> ScanAsync(
        string inventoryFilePath,
        string sourceVersion,
        string targetVersion,
        CancellationToken cancellationToken = default);
}
