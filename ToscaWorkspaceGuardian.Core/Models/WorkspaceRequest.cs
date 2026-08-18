// <copyright file="WorkspaceRequest.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Models;

/// <summary>

/// TODO: Describe WorkspaceRequest.

/// </summary>

public class WorkspaceRequest
{
    public string WorkspacePath { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;

    public bool IsManagedRepository { get; set; }

    public string SourceVersion { get; set; } = string.Empty;

    public string TargetVersion { get; set; } = string.Empty;

    public string OutputFolder { get; set; } = string.Empty;
}

