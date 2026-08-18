namespace ToscaWorkspaceGuardian.Core.Upgrade;

public interface IInventoryUpgradeRuleCatalog
{
    IReadOnlyList<InventoryUpgradeRuleDefinition> GetRules(string sourceVersion, string targetVersion);
}
