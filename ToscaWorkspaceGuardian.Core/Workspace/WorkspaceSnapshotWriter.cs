// <copyright file="WorkspaceSnapshotWriter.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Workspace;

using System.Text.Json;
using ToscaWorkspaceGuardian.Core.Business;
using ToscaWorkspaceGuardian.Core.Interfaces;

/// <summary>

/// TODO: Describe WorkspaceSnapshotWriter.

/// </summary>

public class WorkspaceSnapshotWriter : IWorkspaceSnapshotWriter
{
    public async Task WriteAsync(
        WorkspaceSnapshot snapshot,
        string filePath,
        CancellationToken cancellationToken = default)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
        };

        var json =
            JsonSerializer.Serialize(snapshot, options);

        await File.WriteAllTextAsync(
            filePath,
            json,
            cancellationToken);
    }
}

