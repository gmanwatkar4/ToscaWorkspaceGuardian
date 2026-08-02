// <copyright file="NodePathResolver.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.TCShell;

using ToscaWorkspaceGuardian.Core.Business;
using ToscaWorkspaceGuardian.Core.Interfaces;

/// <summary>

/// TODO: Describe NodePathResolver.

/// </summary>

public class NodePathResolver : INodePathResolver
{
    public string GetNodePath(RepositoryObject obj)
    {
        return obj.NodePath;
    }
}

