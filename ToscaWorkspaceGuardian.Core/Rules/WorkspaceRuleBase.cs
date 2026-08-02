using ToscaWorkspaceGuardian.Core.Business;
using ToscaWorkspaceGuardian.Core.Health;

namespace ToscaWorkspaceGuardian.Core.Rules;

public abstract class WorkspaceRuleBase : IWorkspaceRule
{
    public abstract string RuleId { get; }

    public abstract void Evaluate(
        ParsedWorkspace workspace,
        WorkspaceHealthReport report);

    protected void AddFinding(
        WorkspaceHealthReport report,
        FindingSeverity severity,
        string title,
        string description,
        string recommendation,
        int scorePenalty)
    {
        report.Findings.Add(
            new WorkspaceFinding
            {
                RuleId = RuleId,
                Severity = severity,
                Title = title,
                Description = description,
                Recommendation = recommendation
            });

        report.HealthScore -= scorePenalty;
    }
}