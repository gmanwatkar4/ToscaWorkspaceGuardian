using ToscaWorkspaceGuardian.Core.Business;
using ToscaWorkspaceGuardian.Core.Health;

namespace ToscaWorkspaceGuardian.Core.Rules;

public class WG002_NoUsersRule : WorkspaceRuleBase
{
    public override string RuleId => "WG002";

    public override void Evaluate(
        ParsedWorkspace workspace,
        WorkspaceHealthReport report)
    {
        if (workspace.Users.Count > 0)
            return;

        AddFinding(
            report,
            FindingSeverity.Warning,
            "No Users",
            "Workspace does not contain any users.",
            "Verify repository permissions.",
            10);
    }
}