// <copyright file="RepositoryStatisticsBuilder.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Health;

using ToscaWorkspaceGuardian.Core.Business;

public class RepositoryStatisticsBuilder
{
    public RepositoryStatistics Build(
    WorkspaceSnapshot snapshot,
    IReadOnlyCollection<HealthIssue> issues)
    {
        var stats = new RepositoryStatistics();

        stats.TotalObjects = snapshot.Objects.Count;

        foreach (var obj in snapshot.Objects)
        {
            string path = obj.NodePath ?? string.Empty;

            if (obj.ObjectType == "TCFolder")
            {
                stats.FolderCount++;
            }

            if (path.StartsWith("/Modules/", StringComparison.OrdinalIgnoreCase))
            {
                stats.ModuleCount++;
            }

            if (path.StartsWith("/TestCases/", StringComparison.OrdinalIgnoreCase))
            {
                stats.TestCaseCount++;
            }

            if (path.StartsWith("/Execution/", StringComparison.OrdinalIgnoreCase))
            {
                stats.ExecutionListCount++;
            }

            if (path.StartsWith("/Requirements/", StringComparison.OrdinalIgnoreCase))
            {
                stats.RequirementCount++;
            }

            if (obj.ObjectType == "TCUser")
            {
                stats.UserCount++;
            }

            if (obj.ObjectType == "TCUserGroup")
            {
                stats.GroupCount++;
            }
        }

        stats.HealthIssueCount = issues.Count;

        int score = 100;

        foreach (var issue in issues)
        {
            switch (issue.Severity)
            {
                case "High":
                    score -= 15;
                    break;

                case "Medium":
                    score -= 8;
                    break;

                case "Low":
                    score -= 3;
                    break;

                case "Info":
                    break;
            }
        }

        stats.HealthScore = Math.Max(0, score);

        stats.RuleCounts = issues
        .GroupBy(x => x.RuleId)
        .ToDictionary(
        g => g.Key,
        g => g.Count());

        stats.GeneratedOn = DateTime.Now;

        return stats;
    }
}
