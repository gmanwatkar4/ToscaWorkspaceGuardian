// <copyright file="WorkspaceHealthReport.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Health;

/// <summary>

/// TODO: Describe WorkspaceHealthReport.

/// </summary>

public class WorkspaceHealthReport
{
    public int HealthScore { get; set; } = 100;

    public List<WorkspaceFinding> Findings { get; set; } = new();
}

