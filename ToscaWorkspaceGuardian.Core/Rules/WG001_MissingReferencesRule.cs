using ToscaWorkspaceGuardian.Core.Business;
using ToscaWorkspaceGuardian.Core.Health;

namespace ToscaWorkspaceGuardian.Core.Rules;

public class WG001_MissingReferencesRule : WorkspaceRuleBase
{
    public override string RuleId => "WG001";

    public override void Evaluate(
        ParsedWorkspace workspace,
        WorkspaceHealthReport report)
    {
        if (!workspace.HasMissingReferences)
            return;

        AddFinding(
            report,
            FindingSeverity.Critical,
            "Missing References",
            "Workspace contains missing references.",
            "Resolve all missing references before upgrading.",
            40);
    }
}