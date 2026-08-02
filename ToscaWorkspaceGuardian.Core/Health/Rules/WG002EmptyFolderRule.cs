// <copyright file="WG002EmptyFolderRule.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Health.Rules;

using ToscaWorkspaceGuardian.Core.Business;

/// <summary>

/// TODO: Describe WG002EmptyFolderRule.

/// </summary>

public class WG002EmptyFolderRule : IHealthRule
{
    public string RuleId => "WG002";

    public string Title => "Empty Folder";

    public IEnumerable<HealthIssue> Evaluate(
        WorkspaceSnapshot snapshot)
    {
        foreach (var obj in snapshot.Objects)
        {
            if (obj.ObjectType != "TCFolder")
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(obj.ParentPath))
            {
                continue;
            }

            // Ignore repository root folders
            if (obj.ParentPath == "/")
            {
                continue;
            }

            // Ignore Tosca system folders
            if (obj.NodePath.StartsWith("/Execution/ExecutionLists", StringComparison.OrdinalIgnoreCase) ||
                obj.NodePath.StartsWith("/Execution/TestEvents", StringComparison.OrdinalIgnoreCase) ||
                obj.NodePath.StartsWith("/Execution/Configurations", StringComparison.OrdinalIgnoreCase) ||
                obj.NodePath.StartsWith("/Execution/Exploratory Testing", StringComparison.OrdinalIgnoreCase) ||
                obj.NodePath.StartsWith("/Execution/Interactive Testing", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (!obj.Collections.TryGetValue("Items", out var items))
            {
                continue;
            }

            if (items.Count != 0)
            {
                continue;
            }

            yield return new HealthIssue
            {
                RuleId = this.RuleId,
                Title = this.Title,
                Severity = "Low",
                ObjectName = obj.Name,
                NodePath = obj.NodePath,
                Description = "Folder contains no child objects.",
            };
        }
    }
}

