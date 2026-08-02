// <copyright file="IWorkspaceCrawler.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Interfaces;

using ToscaWorkspaceGuardian.Core.Business;
using ToscaWorkspaceGuardian.Core.Models;

public interface IWorkspaceCrawler
{
    Task<WorkspaceSnapshot> CrawlAsync(
        WorkspaceRequest request,
        CancellationToken cancellationToken = default);
}
