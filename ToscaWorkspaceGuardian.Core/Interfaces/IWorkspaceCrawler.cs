// <copyright file="IWorkspaceCrawler.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Interfaces;

using ToscaWorkspaceGuardian.Core.Business;
using ToscaWorkspaceGuardian.Core.Models;

/// <summary>

/// TODO: Describe IWorkspaceCrawler.

/// </summary>

public interface IWorkspaceCrawler
{
    Task<WorkspaceSnapshot> CrawlAsync(
        WorkspaceRequest request,
        IReadOnlySet<string>? knownExpandableNodePaths = null,
        CancellationToken cancellationToken = default);
}

