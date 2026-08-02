// <copyright file="IWorkspaceCompareService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Services;

using ToscaWorkspaceGuardian.Core.Compare;

public interface IWorkspaceCompareService
{
    Task<CompareResult> CompareAsync(
        string oldSnapshotFile,
        string newSnapshotFile,
        CancellationToken cancellationToken = default);
}
