// <copyright file="WorkspaceSnapshot.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Business;

public class WorkspaceSnapshot
{
    public Dictionary<string, RepositoryObject> ByNodePath { get; } =
    new(StringComparer.OrdinalIgnoreCase);

    public Dictionary<string, List<RepositoryObject>> ByObjectType { get; } =
        new(StringComparer.OrdinalIgnoreCase);

    public List<RepositoryObject> Objects { get; } = new();
}
