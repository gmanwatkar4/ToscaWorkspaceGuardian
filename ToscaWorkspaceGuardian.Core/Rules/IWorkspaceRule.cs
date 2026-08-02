// <copyright file="IWorkspaceRule.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Rules;

using ToscaWorkspaceGuardian.Core.Business;
using ToscaWorkspaceGuardian.Core.Health;

/// <summary>

/// TODO: Describe IWorkspaceRule.

/// </summary>

public interface IWorkspaceRule
{
    string RuleId { get; }

    void Evaluate(
        ParsedWorkspace workspace,
        WorkspaceHealthReport report);
}

