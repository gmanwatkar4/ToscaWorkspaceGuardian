// <copyright file="AnalysisResult.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Models;

using ToscaWorkspaceGuardian.Core.Business;

/// <summary>

/// TODO: Describe AnalysisResult.

/// </summary>

public class AnalysisResult
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public WorkspaceSummary Summary { get; set; } = new();

    public int TotalObjects { get; set; }

    public int HealthScore { get; set; }

    public int HealthIssueCount { get; set; }

    public int BlockingIssues { get; set; }

    public int Warnings { get; set; }

    public bool ReadyForUpgrade { get; set; }

    public string SnapshotFile { get; set; } = string.Empty;

    public string HealthReportFile { get; set; } = string.Empty;

    public string HtmlReportFile { get; set; } = string.Empty;

    public IReadOnlyList<HealthIssue> HealthIssues { get; set; } = Array.Empty<HealthIssue>();

    public IReadOnlyList<string> UpgradeRecommendations { get; set; } = Array.Empty<string>();
}

