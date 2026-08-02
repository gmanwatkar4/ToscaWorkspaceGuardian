// <copyright file="WG001MissingReferencesRule.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Health.Rules;

using ToscaWorkspaceGuardian.Core.Business;

public class WG001MissingReferencesRule : IHealthRule
{
    public string RuleId => "WG001";

    public string Title => "Objects with Missing References";

    public IEnumerable<HealthIssue> Evaluate(
        WorkspaceSnapshot snapshot)
    {
        foreach (var obj in snapshot.Objects)
        {
            if (!obj.Properties.TryGetValue(
                    "HasMissingReferences",
                    out var value))
            {
                continue;
            }

            if (!string.Equals(
                    value,
                    "True",
                    StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            yield return new HealthIssue
            {
                RuleId = this.RuleId,
                Title = this.Title,
                Severity = "High",
                ObjectName = obj.Name,
                NodePath = obj.NodePath,
                Description =
                    "Repository object contains missing references.",
            };
        }
    }
}
