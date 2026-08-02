// <copyright file="WG003_NoGroupsRule.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Rules;

using ToscaWorkspaceGuardian.Core.Business;
using ToscaWorkspaceGuardian.Core.Health;

public class WG003_NoGroupsRule : WorkspaceRuleBase
{
    public override string RuleId => "WG003";

    public override void Evaluate(
        ParsedWorkspace workspace,
        WorkspaceHealthReport report)
    {
        if (workspace.Groups.Count > 0)
        {
            return;
        }

        this.AddFinding(
            report,
            FindingSeverity.Warning,
            "No Groups",
            "Workspace does not contain any groups.",
            "Verify repository security configuration.",
            10);
    }
}
