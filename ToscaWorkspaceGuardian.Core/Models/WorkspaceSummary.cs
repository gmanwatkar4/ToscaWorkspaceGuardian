// <copyright file="WorkspaceSummary.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Models;

/// <summary>

/// TODO: Describe WorkspaceSummary.

/// </summary>

public class WorkspaceSummary
{
    public WorkspaceInfo WorkspaceInfo { get; set; } = new();

    public WorkspaceMetadata Metadata { get; set; } = new();

    public WorkspaceStatistics Statistics { get; set; } = new();
}

