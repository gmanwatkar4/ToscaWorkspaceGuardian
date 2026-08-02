// <copyright file="WorkspaceFinding.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Health;

public class WorkspaceFinding
{
    public FindingSeverity Severity { get; set; }

    public string RuleId { get; set; } = string.Empty;

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Recommendation { get; set; } = string.Empty;
}
