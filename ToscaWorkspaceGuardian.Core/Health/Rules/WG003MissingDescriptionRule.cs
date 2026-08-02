using ToscaWorkspaceGuardian.Core.Business;

namespace ToscaWorkspaceGuardian.Core.Health.Rules;

public class WG003MissingDescriptionRule : IHealthRule
{
    public string RuleId => "WG003";

    public string Title => "Missing Description";

    public IEnumerable<HealthIssue> Evaluate(
        WorkspaceSnapshot snapshot)
    {
        foreach (var obj in snapshot.Objects)
        {
            // Skip repository roots
            if (obj.ParentPath == "/")
                continue;

            // Skip Standard Modules
            if (obj.NodePath.Contains("/Standard modules/", StringComparison.OrdinalIgnoreCase))
                continue;

            // Skip Engine objects
            if (obj.NodePath.Contains("/Engines/", StringComparison.OrdinalIgnoreCase))
                continue;

            yield return new HealthIssue
            {
                RuleId = RuleId,
                Title = Title,
                Severity = "Info",
                ObjectName = obj.Name,
                NodePath = obj.NodePath,
                Description = "Description is empty."
            };
        }
    }
}