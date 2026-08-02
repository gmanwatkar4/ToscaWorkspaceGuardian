// <copyright file="WorkspaceHealthAnalyzer.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Health;

using ToscaWorkspaceGuardian.Core.Business;
using ToscaWorkspaceGuardian.Core.Interfaces;

/// <summary>

/// TODO: Describe WorkspaceHealthAnalyzer.

/// </summary>

public class WorkspaceHealthAnalyzer : IWorkspaceHealthAnalyzer
{
    public WorkspaceHealthReport Analyze(
        ParsedWorkspace workspace)
    {
        var report = new WorkspaceHealthReport
        {
            HealthScore = 100,
        };

        //--------------------------------------------------
        // WG001 - Missing References
        //--------------------------------------------------
        if (workspace.HasMissingReferences)
        {
            report.Findings.Add(new WorkspaceFinding
            {
                RuleId = "WG001",
                Severity = FindingSeverity.Critical,
                Title = "Missing References",
                Description = "Workspace contains missing references.",
                Recommendation = "Resolve all missing references before upgrading.",
            });

            report.HealthScore -= 40;
        }

        //--------------------------------------------------
        // WG002 - No Users
        //--------------------------------------------------
        if (workspace.Users.Count == 0)
        {
            report.Findings.Add(new WorkspaceFinding
            {
                RuleId = "WG002",
                Severity = FindingSeverity.Warning,
                Title = "No Users",
                Description = "Workspace does not contain any users.",
                Recommendation = "Verify repository permissions.",
            });

            report.HealthScore -= 10;
        }

        //--------------------------------------------------
        // WG003 - No Groups
        //--------------------------------------------------
        if (workspace.Groups.Count == 0)
        {
            report.Findings.Add(new WorkspaceFinding
            {
                RuleId = "WG003",
                Severity = FindingSeverity.Warning,
                Title = "No Groups",
                Description = "Workspace does not contain any groups.",
                Recommendation = "Verify repository security configuration.",
            });

            report.HealthScore -= 10;
        }

        //--------------------------------------------------
        // WG004 - Empty Workspace
        //--------------------------------------------------
        if (workspace.RootFolders.Count == 0)
        {
            report.Findings.Add(new WorkspaceFinding
            {
                RuleId = "WG004",
                Severity = FindingSeverity.Warning,
                Title = "No Root Folders",
                Description = "Workspace does not contain any root folders.",
                Recommendation = "Verify repository integrity.",
            });

            report.HealthScore -= 20;
        }

        //--------------------------------------------------
        // WG005 - Revision Check
        //--------------------------------------------------
        if (workspace.Revision <= 0)
        {
            report.Findings.Add(new WorkspaceFinding
            {
                RuleId = "WG005",
                Severity = FindingSeverity.Information,
                Title = "Revision Information",
                Description = "Workspace revision is unavailable.",
                Recommendation = "Verify repository metadata.",
            });

            report.HealthScore -= 5;
        }

        //--------------------------------------------------
        // Normalize Score
        //--------------------------------------------------
        if (report.HealthScore < 0)
        {
            report.HealthScore = 0;
        }

        if (report.HealthScore > 100)
        {
            report.HealthScore = 100;
        }

        return report;
    }
}

