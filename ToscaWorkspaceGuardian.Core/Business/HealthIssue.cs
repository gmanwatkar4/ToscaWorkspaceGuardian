// <copyright file="HealthIssue.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Business;

/// <summary>

/// TODO: Describe HealthIssue.

/// </summary>

public class HealthIssue
{
    public string RuleId { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Severity { get; set; } = "Info";

    public string ObjectName { get; set; } = string.Empty;

    public string NodePath { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}

