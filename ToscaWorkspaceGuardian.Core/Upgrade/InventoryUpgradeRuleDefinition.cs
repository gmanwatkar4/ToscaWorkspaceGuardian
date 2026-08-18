namespace ToscaWorkspaceGuardian.Core.Upgrade;

public sealed class InventoryUpgradeRuleDefinition
{
    public string RuleId { get; init; } = string.Empty;
    public string SourceVersion { get; init; } = string.Empty;
    public string TargetVersion { get; init; } = string.Empty;
    public string Severity { get; init; } = "Advisory";
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string RecommendedAction { get; init; } = string.Empty;
    public string DocumentationUrl { get; init; } = string.Empty;
    public string ObjectType { get; init; } = string.Empty;
    public List<string> Names { get; init; } = new();
    public string MatchMode { get; init; } = string.Empty;
    public string Contains { get; init; } = string.Empty;
}
