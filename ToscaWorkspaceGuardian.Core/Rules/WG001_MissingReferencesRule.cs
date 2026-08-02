// <copyright file="WG001_MissingReferencesRule.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Rules;

using ToscaWorkspaceGuardian.Core.Business;
using ToscaWorkspaceGuardian.Core.Health;

/// <summary>

/// TODO: Describe WG001_MissingReferencesRule.

/// </summary>

public class WG001_MissingReferencesRule : WorkspaceRuleBase
{
    public override string RuleId => "WG001";

    public override void Evaluate(
        ParsedWorkspace workspace,
        WorkspaceHealthReport report)
    {
        if (!workspace.HasMissingReferences)
        {
            return;
        }

        this.AddFinding(
            report,
            FindingSeverity.Critical,
            "Missing References",
            "Workspace contains missing references.",
            "Resolve all missing references before upgrading.",
            40);
    }
}

