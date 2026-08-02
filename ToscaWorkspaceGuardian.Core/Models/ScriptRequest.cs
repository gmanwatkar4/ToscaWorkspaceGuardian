// <copyright file="ScriptRequest.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Models;

/// <summary>

/// TODO: Describe ScriptRequest.

/// </summary>

public class ScriptRequest
{
    public ScriptType ScriptType { get; set; }

    public WorkspaceRequest WorkspaceRequest { get; set; } = new();

    public List<string> Queries { get; set; } = new();
}

