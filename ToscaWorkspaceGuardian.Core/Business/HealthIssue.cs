namespace ToscaWorkspaceGuardian.Core.Business;

public class HealthIssue
{
    public string RuleId { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Severity { get; set; } = "Info";

    public string ObjectName { get; set; } = string.Empty;

    public string NodePath { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}