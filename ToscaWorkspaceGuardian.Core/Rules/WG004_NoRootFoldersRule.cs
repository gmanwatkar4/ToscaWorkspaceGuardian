// <copyright file="WG004_NoRootFoldersRule.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Rules;

using ToscaWorkspaceGuardian.Core.Business;
using ToscaWorkspaceGuardian.Core.Health;

public class WG004_NoRootFoldersRule : WorkspaceRuleBase
{
    public override string RuleId => "WG004";

    public override void Evaluate(
        ParsedWorkspace workspace,
        WorkspaceHealthReport report)
    {
        if (workspace.RootFolders.Count > 0)
        {
            return;
        }

        this.AddFinding(
            report,
            FindingSeverity.Warning,
            "Empty Workspace",
            "Workspace does not contain any root folders.",
            "Verify repository integrity.",
            20);
    }
}
