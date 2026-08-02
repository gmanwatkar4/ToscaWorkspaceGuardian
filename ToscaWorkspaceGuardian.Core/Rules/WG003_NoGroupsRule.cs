using ToscaWorkspaceGuardian.Core.Business;
using ToscaWorkspaceGuardian.Core.Health;

namespace ToscaWorkspaceGuardian.Core.Rules;

public class WG003_NoGroupsRule : WorkspaceRuleBase
{
    public override string RuleId => "WG003";

    public override void Evaluate(
        ParsedWorkspace workspace,
        WorkspaceHealthReport report)
    {
        if (workspace.Groups.Count > 0)
            return;

        AddFinding(
            report,
            FindingSeverity.Warning,
            "No Groups",
            "Workspace does not contain any groups.",
            "Verify repository security configuration.",
            10);
    }
}