namespace ToscaWorkspaceGuardian.Core.Upgrade;

using System.Text.Json;

public sealed class InventoryUpgradeRuleCatalog : IInventoryUpgradeRuleCatalog
{
    private const string CatalogFileName = "UpgradeRules.2025.1-to-2026.1.json";
    private readonly Lazy<IReadOnlyList<InventoryUpgradeRuleDefinition>> rules = new(LoadRules);

    public IReadOnlyList<InventoryUpgradeRuleDefinition> GetRules(string sourceVersion, string targetVersion) =>
        this.rules.Value.Where(rule =>
                string.Equals(rule.SourceVersion, sourceVersion, StringComparison.OrdinalIgnoreCase)
                && string.Equals(rule.TargetVersion, targetVersion, StringComparison.OrdinalIgnoreCase))
            .ToList();

    private static IReadOnlyList<InventoryUpgradeRuleDefinition> LoadRules()
    {
        var filePath = Path.Combine(AppContext.BaseDirectory, "Rules", CatalogFileName);
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("The bundled upgrade-rule catalog could not be found.", filePath);
        }

        var document = JsonSerializer.Deserialize<InventoryUpgradeRuleDocument>(
            File.ReadAllText(filePath),
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            });
        return document?.Rules ?? new List<InventoryUpgradeRuleDefinition>();
    }

    private sealed class InventoryUpgradeRuleDocument
    {
        public List<InventoryUpgradeRuleDefinition> Rules { get; init; } = new();
    }
}
