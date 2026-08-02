// <copyright file="HealthAnalyzer.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Health;

using ToscaWorkspaceGuardian.Core.Business;

/// <summary>

/// TODO: Describe HealthAnalyzer.

/// </summary>

public class HealthAnalyzer
{
    private readonly IEnumerable<IHealthRule> rules;

    public HealthAnalyzer(
        IEnumerable<IHealthRule> rules)
    {
        this.rules = rules;
    }

    public List<HealthIssue> Analyze(
        WorkspaceSnapshot snapshot)
    {
        var issues = new List<HealthIssue>();

        foreach (var rule in this.rules)
        {
            issues.AddRange(rule.Evaluate(snapshot));
        }

        return issues;
    }
}

