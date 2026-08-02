// <copyright file="IWorkspaceReader.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Interfaces;

using ToscaWorkspaceGuardian.Core.Models;

/// <summary>

/// TODO: Describe IWorkspaceReader.

/// </summary>

public interface IWorkspaceReader
{
    Task<WorkspaceSummary> ReadAsync(
        WorkspaceRequest request,
        CancellationToken cancellationToken = default);
}

