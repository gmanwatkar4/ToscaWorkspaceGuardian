// <copyright file="IRepositoryTreeWalker.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Interfaces;

using ToscaWorkspaceGuardian.Core.Business;
using ToscaWorkspaceGuardian.Core.Models;

/// <summary>

/// TODO: Describe IRepositoryTreeWalker.

/// </summary>

public interface IRepositoryTreeWalker
{
    Task<ParsedWorkspace> ScanAsync(
        WorkspaceRequest request,
        CancellationToken cancellationToken = default);
}

