// <copyright file="IWorkspaceSnapshotExporter.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Interfaces;

using ToscaWorkspaceGuardian.Core.Business;

/// <summary>

/// TODO: Describe IWorkspaceSnapshotExporter.

/// </summary>

public interface IWorkspaceSnapshotExporter
{
    Task ExportAsync(
        WorkspaceSnapshot snapshot,
        string outputFile,
        CancellationToken cancellationToken = default);
}

