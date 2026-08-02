// <copyright file="WorkspaceSnapshotExporter.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Export;

using System.Text.Json;
using ToscaWorkspaceGuardian.Core.Business;
using ToscaWorkspaceGuardian.Core.Interfaces;

/// <summary>

/// TODO: Describe WorkspaceSnapshotExporter.

/// </summary>

public class WorkspaceSnapshotExporter : IWorkspaceSnapshotExporter
{
    public async Task ExportAsync(
        WorkspaceSnapshot snapshot,
        string outputFile,
        CancellationToken cancellationToken = default)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
        };

        var json = JsonSerializer.Serialize(snapshot, options);

        Directory.CreateDirectory(
            Path.GetDirectoryName(outputFile)!);

        await File.WriteAllTextAsync(
            outputFile,
            json,
            cancellationToken);
    }
}

