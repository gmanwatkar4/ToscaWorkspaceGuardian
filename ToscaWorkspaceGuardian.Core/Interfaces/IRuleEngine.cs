// <copyright file="IRuleEngine.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Interfaces;

using ToscaWorkspaceGuardian.Core.Models;

public interface IRuleEngine
{
    Task<AnalysisResult> ExecuteAsync(
        WorkspaceSummary workspace,
        CancellationToken cancellationToken = default);
}
