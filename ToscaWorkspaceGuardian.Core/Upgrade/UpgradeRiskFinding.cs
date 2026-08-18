namespace ToscaWorkspaceGuardian.Core.Upgrade;

public sealed record UpgradeRiskFinding(
    string RuleId,
    string Severity,
    string Title,
    string ObjectName,
    string ObjectType,
    string Description,
    string RecommendedAction,
    string TqlQuery);
