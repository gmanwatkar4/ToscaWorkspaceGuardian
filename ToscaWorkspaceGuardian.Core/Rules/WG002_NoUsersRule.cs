// <copyright file="WG002_NoUsersRule.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Rules;

using ToscaWorkspaceGuardian.Core.Business;
using ToscaWorkspaceGuardian.Core.Health;

/// <summary>

/// TODO: Describe WG002_NoUsersRule.

/// </summary>

public class WG002_NoUsersRule : WorkspaceRuleBase
{
    public override string RuleId => "WG002";

    public override void Evaluate(
        ParsedWorkspace workspace,
        WorkspaceHealthReport report)
    {
        if (workspace.Users.Count > 0)
        {
            return;
        }

        this.AddFinding(
            report,
            FindingSeverity.Warning,
            "No Users",
            "Workspace does not contain any users.",
            "Verify repository permissions.",
            10);
    }
}

