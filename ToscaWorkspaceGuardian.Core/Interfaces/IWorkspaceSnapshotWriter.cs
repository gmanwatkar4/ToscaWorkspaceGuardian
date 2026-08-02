// <copyright file="IWorkspaceSnapshotWriter.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Interfaces;

using ToscaWorkspaceGuardian.Core.Business;

/// <summary>

/// TODO: Describe IWorkspaceSnapshotWriter.

/// </summary>

public interface IWorkspaceSnapshotWriter
{
    Task WriteAsync(
        WorkspaceSnapshot snapshot,
        string filePath,
        CancellationToken cancellationToken = default);
}

