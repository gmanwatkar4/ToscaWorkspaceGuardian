// <copyright file="WorkspaceCompareService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Services;

using System.Text.Json;
using ToscaWorkspaceGuardian.Core.Business;
using ToscaWorkspaceGuardian.Core.Compare;

public class WorkspaceCompareService : IWorkspaceCompareService
{
    private readonly WorkspaceSnapshotComparer comparer;

    public WorkspaceCompareService(
        WorkspaceSnapshotComparer comparer)
    {
        this.comparer = comparer;
    }

    public async Task<CompareResult> CompareAsync(
        string oldSnapshotFile,
        string newSnapshotFile,
        CancellationToken cancellationToken = default)
    {
        var oldJson = await File.ReadAllTextAsync(
            oldSnapshotFile,
            cancellationToken);

        var newJson = await File.ReadAllTextAsync(
            newSnapshotFile,
            cancellationToken);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        var oldSnapshot =
            JsonSerializer.Deserialize<WorkspaceSnapshot>(
                oldJson,
                options)
            ?? new WorkspaceSnapshot();

        var newSnapshot =
            JsonSerializer.Deserialize<WorkspaceSnapshot>(
                newJson,
                options)
            ?? new WorkspaceSnapshot();

        return this.comparer.Compare(
            oldSnapshot,
            newSnapshot);
    }
}
