// <copyright file="WorkspaceRuleBase.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Rules;

using ToscaWorkspaceGuardian.Core.Business;
using ToscaWorkspaceGuardian.Core.Health;

public abstract class WorkspaceRuleBase : IWorkspaceRule
{
    public abstract string RuleId { get; }

    public abstract void Evaluate(
        ParsedWorkspace workspace,
        WorkspaceHealthReport report);

    protected void AddFinding(
        WorkspaceHealthReport report,
        FindingSeverity severity,
        string title,
        string description,
        string recommendation,
        int scorePenalty)
    {
        report.Findings.Add(
            new WorkspaceFinding
            {
                RuleId = this.RuleId,
                Severity = severity,
                Title = title,
                Description = description,
                Recommendation = recommendation,
            });

        report.HealthScore -= scorePenalty;
    }
}
