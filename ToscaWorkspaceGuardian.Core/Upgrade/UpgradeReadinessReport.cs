namespace ToscaWorkspaceGuardian.Core.Upgrade;

public class UpgradeReadinessReport
{
    public string SourceVersion { get; set; } = "";

    public string TargetVersion { get; set; } = "";

    public int HealthScore { get; set; }

    public int BlockingIssues { get; set; }

    public int Warnings { get; set; }

    public bool ReadyForUpgrade { get; set; }

    public List<string> Recommendations { get; set; } = new();

    public List<VersionCompatibilityRule> CompatibilityRules { get; set; } = new();
}