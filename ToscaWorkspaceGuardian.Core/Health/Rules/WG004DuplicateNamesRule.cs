using ToscaWorkspaceGuardian.Core.Business;

namespace ToscaWorkspaceGuardian.Core.Health.Rules;

public class WG004DuplicateNamesRule : IHealthRule
{
    public string RuleId => "WG004";

    public string Title => "Duplicate Object Names";

    public IEnumerable<HealthIssue> Evaluate(
    WorkspaceSnapshot snapshot)
    {
        var duplicateGroups = snapshot.Objects
            .GroupBy(x => new
            {
                x.ParentPath,
                x.Name,
                x.ObjectType
            })
            .Where(g => g.Count() > 1);

        foreach (var group in duplicateGroups)
        {
            foreach (var obj in group)
            {
                yield return new HealthIssue
                {
                    RuleId = RuleId,
                    Title = Title,
                    Severity = "Medium",
                    ObjectName = obj.Name,
                    NodePath = obj.NodePath,
                    Description =
                        $"Duplicate sibling object ({group.Count()} occurrences)."
                };
            }
        }
    }
}