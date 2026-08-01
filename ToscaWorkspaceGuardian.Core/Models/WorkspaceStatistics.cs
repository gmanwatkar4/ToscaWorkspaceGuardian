namespace ToscaWorkspaceGuardian.Core.Models;

public class WorkspaceStatistics
{
    public int ModuleCount { get; set; }

    public int TestCaseCount { get; set; }

    public int TestStepCount { get; set; }

    public int ExecutionListCount { get; set; }

    public int RequirementCount { get; set; }

    public int FolderCount { get; set; }
}