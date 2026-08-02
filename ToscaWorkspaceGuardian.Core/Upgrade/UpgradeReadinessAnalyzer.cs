using ToscaWorkspaceGuardian.Core.Business;

namespace ToscaWorkspaceGuardian.Core.Upgrade;

public class UpgradeReadinessAnalyzer
{
    private readonly VersionCompatibilityEngine _engine;

    public UpgradeReadinessAnalyzer(
        VersionCompatibilityEngine engine)
    {
        _engine = engine;
    }
    public UpgradeReadinessReport Analyze(
        RepositoryStatistics statistics,
        IReadOnlyCollection<HealthIssue> issues,
        string sourceVersion,
        string targetVersion)
    {
        var report = new UpgradeReadinessReport
        {
            SourceVersion = sourceVersion,
            TargetVersion = targetVersion,
            HealthScore = statistics.HealthScore
        };

        foreach (var issue in issues)
        {
            switch (issue.Severity)
            {
                case "High":
                    report.BlockingIssues++;
                    break;

                case "Medium":
                    report.Warnings++;
                    break;
            }
        }

        if (issues.Any(x => x.RuleId == "WG002"))
        {
            report.Recommendations.Add(
                "Review and remove unnecessary empty folders.");
        }

        if (issues.Any(x => x.RuleId == "WG003"))
        {
            report.Recommendations.Add(
                "Consider adding descriptions to business objects.");
        }

        if (issues.Any(x => x.RuleId == "WG004"))
        {
            report.Recommendations.Add(
                "Review duplicate sibling object names.");
        }

        report.ReadyForUpgrade =
            report.BlockingIssues == 0;

        report.CompatibilityRules =
        _engine.GetRules(
        sourceVersion,
        targetVersion)
        .ToList();

        return report;
    }
}