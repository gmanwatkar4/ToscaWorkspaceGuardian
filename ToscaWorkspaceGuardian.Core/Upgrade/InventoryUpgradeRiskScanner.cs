namespace ToscaWorkspaceGuardian.Core.Upgrade;

using System.Text.Json;
using ToscaWorkspaceGuardian.Core.Workspace;

/// <summary>Scans a saved inventory only; it never changes the Tosca workspace.</summary>
public sealed class InventoryUpgradeRiskScanner : IInventoryUpgradeRiskScanner
{
    private readonly IInventoryUpgradeRuleCatalog ruleCatalog;

    public InventoryUpgradeRiskScanner(IInventoryUpgradeRuleCatalog ruleCatalog)
    {
        this.ruleCatalog = ruleCatalog;
    }

    public async Task<UpgradeRiskScanResult> ScanAsync(
        string inventoryFilePath,
        string sourceVersion,
        string targetVersion,
        CancellationToken cancellationToken = default)
    {
        if (!File.Exists(inventoryFilePath))
        {
            throw new FileNotFoundException("Create a workspace inventory before evaluating upgrade rules.", inventoryFilePath);
        }

        var inventory = JsonSerializer.Deserialize<WorkspaceInventorySnapshot>(
            await File.ReadAllTextAsync(inventoryFilePath, cancellationToken))
            ?? throw new InvalidDataException("The workspace inventory JSON could not be read.");
        var rules = this.ruleCatalog.GetRules(sourceVersion, targetVersion);
        if (rules.Count == 0)
        {
            return new UpgradeRiskScanResult { Diagnostics = new[] { "No documented inventory rules are bundled for the selected upgrade path yet." } };
        }

        var findings = new List<UpgradeRiskFinding>();
        foreach (var rule in rules)
        {
            foreach (var node in inventory.Nodes.Where(node => Matches(node, rule)))
            {
                findings.Add(new UpgradeRiskFinding(
                    rule.RuleId, rule.Severity, rule.Title, node.Name, node.ObjectType,
                    rule.Description, rule.RecommendedAction,
                    $"Inventory node: {node.NodePath} | Documentation: {rule.DocumentationUrl}"));
            }
        }

        return new UpgradeRiskScanResult { Findings = findings };
    }

    private static bool Matches(WorkspaceInventoryNode node, InventoryUpgradeRuleDefinition rule)
    {
        if (!string.IsNullOrWhiteSpace(rule.ObjectType)
            && !string.Equals(node.ObjectType, rule.ObjectType, StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return rule.MatchMode switch
        {
            "ExactName" => rule.Names.Any(name => string.Equals(node.Name, name, StringComparison.OrdinalIgnoreCase)),
            "NameOrPropertyContains" => node.Name.Contains(rule.Contains, StringComparison.OrdinalIgnoreCase)
                || node.Properties.Any(property => property.Key.Contains(rule.Contains, StringComparison.OrdinalIgnoreCase)
                    || property.Value.Contains(rule.Contains, StringComparison.OrdinalIgnoreCase)),
            _ => false,
        };
    }
}
