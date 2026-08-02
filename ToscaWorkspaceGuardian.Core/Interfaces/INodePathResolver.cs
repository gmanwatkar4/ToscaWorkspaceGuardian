// <copyright file="INodePathResolver.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Interfaces;

using ToscaWorkspaceGuardian.Core.Business;

/// <summary>

/// TODO: Describe INodePathResolver.

/// </summary>

public interface INodePathResolver
{
    string GetNodePath(RepositoryObject obj);
}

