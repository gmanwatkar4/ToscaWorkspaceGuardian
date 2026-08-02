namespace ToscaWorkspaceGuardian.Core.Upgrade;

public class VersionCompatibilityRule
{
    public string SourceVersion { get; set; } = "";

    public string TargetVersion { get; set; } = "";

    public string Recommendation { get; set; } = "";

    public string Severity { get; set; } = "Info";
}