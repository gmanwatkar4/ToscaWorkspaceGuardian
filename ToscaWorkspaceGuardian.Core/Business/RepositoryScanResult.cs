// <copyright file="RepositoryScanResult.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Business;

/// <summary>

/// TODO: Describe RepositoryScanResult.

/// </summary>

public class RepositoryScanResult
{
    public List<RepositoryNode> Nodes { get; } = new();

    public ParsedWorkspace Workspace { get; } = new();
}

