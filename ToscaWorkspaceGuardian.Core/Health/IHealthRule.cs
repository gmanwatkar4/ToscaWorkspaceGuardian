using ToscaWorkspaceGuardian.Core.Business;

namespace ToscaWorkspaceGuardian.Core.Health;

public interface IHealthRule
{
    string RuleId { get; }

    string Title { get; }

    IEnumerable<HealthIssue> Evaluate(
        WorkspaceSnapshot snapshot);
}