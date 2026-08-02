using ToscaWorkspaceGuardian.Core.Business;
using ToscaWorkspaceGuardian.Core.Health;

namespace ToscaWorkspaceGuardian.Core.Rules;

public interface IWorkspaceRule
{
    string RuleId { get; }

    void Evaluate(
        ParsedWorkspace workspace,
        WorkspaceHealthReport report);
}