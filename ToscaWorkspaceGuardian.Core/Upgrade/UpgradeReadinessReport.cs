// <copyright file="UpgradeReadinessReport.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Upgrade;

public class UpgradeReadinessReport
{
    public string SourceVersion { get; set; } = string.Empty;

    public string TargetVersion { get; set; } = string.Empty;

    public int HealthScore { get; set; }

    public int BlockingIssues { get; set; }

    public int Warnings { get; set; }

    public bool ReadyForUpgrade { get; set; }

    public List<string> Recommendations { get; set; } = new();

    public List<VersionCompatibilityRule> CompatibilityRules { get; set; } = new();
}
