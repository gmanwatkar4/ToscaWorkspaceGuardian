// <copyright file="WorkspaceMetadata.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Models;

public class WorkspaceMetadata
{
    public string WorkspaceId { get; set; } = string.Empty;

    public string CommonRepositoryId { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string ToscaVersion { get; set; } = string.Empty;

    public DateTime AnalysisTime { get; set; } = DateTime.Now;
}
