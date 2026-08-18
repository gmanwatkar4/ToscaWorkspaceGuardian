// Copyright (c) PlaceholderCompany. All rights reserved.

namespace ToscaWorkspaceGuardian.Core.Rules;

using ToscaWorkspaceGuardian.Core.Business;
using ToscaWorkspaceGuardian.Core.Health;

/// <summary>Reports a workspace that has no configured users.</summary>
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

#pragma warning disable IDE0003 // StyleCop requires explicit instance qualification for inherited calls.
        this.AddFinding(
#pragma warning restore IDE0003
            report,
            FindingSeverity.Warning,
            "No Users",
            "Workspace does not contain any users.",
            "Verify repository permissions.",
            10);
    }
}
