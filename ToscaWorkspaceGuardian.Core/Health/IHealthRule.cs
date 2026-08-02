// <copyright file="IHealthRule.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Health;

using ToscaWorkspaceGuardian.Core.Business;

public interface IHealthRule
{
    string RuleId { get; }

    string Title { get; }

    IEnumerable<HealthIssue> Evaluate(
        WorkspaceSnapshot snapshot);
}
