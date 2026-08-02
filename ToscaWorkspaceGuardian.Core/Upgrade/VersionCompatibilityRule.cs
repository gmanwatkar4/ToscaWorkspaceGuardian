// <copyright file="VersionCompatibilityRule.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Upgrade;

public class VersionCompatibilityRule
{
    public string SourceVersion { get; set; } = string.Empty;

    public string TargetVersion { get; set; } = string.Empty;

    public string Recommendation { get; set; } = string.Empty;

    public string Severity { get; set; } = "Info";
}
