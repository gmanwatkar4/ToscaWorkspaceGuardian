namespace ToscaWorkspaceGuardian.Core.Business;

public class RepositoryStatistics
{
    public int TotalObjects { get; set; }

    public int FolderCount { get; set; }

    public int ModuleCount { get; set; }

    public int TestCaseCount { get; set; }

    public int ExecutionListCount { get; set; }

    public int RequirementCount { get; set; }

    public int UserCount { get; set; }

    public int GroupCount { get; set; }

    public int HealthIssueCount { get; set; }

    public int HealthScore { get; set; }

    public Dictionary<string, int> RuleCounts { get; set; } = new();

    public DateTime GeneratedOn { get; set; } = DateTime.Now;
}