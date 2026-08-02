using ToscaWorkspaceGuardian.Core.Business;
using ToscaWorkspaceGuardian.Core.Health;

namespace ToscaWorkspaceGuardian.Core.Rules;

public class WG004_NoRootFoldersRule : WorkspaceRuleBase
{
    public override string RuleId => "WG004";

    public override void Evaluate(
        ParsedWorkspace workspace,
        WorkspaceHealthReport report)
    {
        if (workspace.RootFolders.Count > 0)
            return;

        AddFinding(
            report,
            FindingSeverity.Warning,
            "Empty Workspace",
            "Workspace does not contain any root folders.",
            "Verify repository integrity.",
            20);
    }
}