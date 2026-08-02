using ToscaWorkspaceGuardian.Core.Business;

namespace ToscaWorkspaceGuardian.Core.Health;

public class HealthAnalyzer
{
    private readonly IEnumerable<IHealthRule> _rules;

    public HealthAnalyzer(
        IEnumerable<IHealthRule> rules)
    {
        _rules = rules;
    }

    public List<HealthIssue> Analyze(
        WorkspaceSnapshot snapshot)
    {
        var issues = new List<HealthIssue>();

        foreach (var rule in _rules)
        {
            issues.AddRange(rule.Evaluate(snapshot));
        }

        return issues;
    }
}