namespace ToscaWorkspaceGuardian.Core.Upgrade;

/// <summary>
/// A version-specific, read-only TQL rule derived from an approved upgrade source.
/// </summary>
public sealed record UpgradeRiskRule(
    string RuleId,
    string Severity,
    string Title,
    string TqlQuery,
    string Description,
    string RecommendedAction);
