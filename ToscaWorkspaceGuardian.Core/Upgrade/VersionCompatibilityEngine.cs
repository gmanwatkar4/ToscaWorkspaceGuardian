namespace ToscaWorkspaceGuardian.Core.Upgrade;

public class VersionCompatibilityEngine
{
    public IReadOnlyCollection<VersionCompatibilityRule> GetRules(
        string sourceVersion,
        string targetVersion)
    {
        var rules = new List<VersionCompatibilityRule>();

        //------------------------------------------
        // Generic Recommendations
        //------------------------------------------

        rules.Add(new VersionCompatibilityRule
        {
            SourceVersion = sourceVersion,
            TargetVersion = targetVersion,
            Severity = "Info",
            Recommendation =
                "Generate a fresh workspace snapshot before upgrading."
        });

        rules.Add(new VersionCompatibilityRule
        {
            SourceVersion = sourceVersion,
            TargetVersion = targetVersion,
            Severity = "Info",
            Recommendation =
                "Run snapshot comparison after upgrade."
        });

        rules.Add(new VersionCompatibilityRule
        {
            SourceVersion = sourceVersion,
            TargetVersion = targetVersion,
            Severity = "Low",
            Recommendation =
                "Execute smoke regression after upgrade."
        });

        //------------------------------------------
        // Example Version Rule
        //------------------------------------------

        if (sourceVersion == "2025.1" &&
            targetVersion == "2026.1")
        {
            rules.Add(new VersionCompatibilityRule
            {
                SourceVersion = sourceVersion,
                TargetVersion = targetVersion,
                Severity = "Medium",
                Recommendation =
                    "Review repository health before upgrading to 2026.1."
            });
        }

        return rules;
    }
}