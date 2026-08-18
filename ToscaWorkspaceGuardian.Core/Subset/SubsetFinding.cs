namespace ToscaWorkspaceGuardian.Core.Subset;

/// <summary>
/// A documented upgrade consideration found in an exported Tosca subset.
/// </summary>
public sealed record SubsetFinding(
    string RuleId,
    string Severity,
    string ObjectName,
    string Description,
    string Recommendation);
