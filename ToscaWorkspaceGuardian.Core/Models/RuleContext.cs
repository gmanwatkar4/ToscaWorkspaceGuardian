// <copyright file="RuleContext.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Models;

public class RuleContext
{
    public WorkspaceSummary Workspace { get; set; } = new();
}
