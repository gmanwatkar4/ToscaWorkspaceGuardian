namespace ToscaWorkspaceGuardian.Core.Health;

public class WorkspaceHealthReport
{
    public int HealthScore { get; set; } = 100;

    public List<WorkspaceFinding> Findings { get; set; } = new();
}