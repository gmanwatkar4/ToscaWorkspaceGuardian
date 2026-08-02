// <copyright file="NodePathResolver.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.TCShell;

using ToscaWorkspaceGuardian.Core.Business;
using ToscaWorkspaceGuardian.Core.Interfaces;

public class NodePathResolver : INodePathResolver
{
    public string GetNodePath(RepositoryObject obj)
    {
        return obj.NodePath;
    }
}
