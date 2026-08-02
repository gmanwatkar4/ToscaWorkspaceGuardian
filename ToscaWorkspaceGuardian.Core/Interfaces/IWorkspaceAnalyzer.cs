// <copyright file="IWorkspaceAnalyzer.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Interfaces;

using ToscaWorkspaceGuardian.Core.Models;

public interface IWorkspaceAnalyzer
{
    Task<AnalysisResult> AnalyzeAsync(
        WorkspaceRequest request,
        CancellationToken cancellationToken = default);
}
