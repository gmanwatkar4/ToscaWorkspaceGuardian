// <copyright file="ParsedWorkspace.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Business;

/// <summary>

/// TODO: Describe ParsedWorkspace.

/// </summary>

public class ParsedWorkspace
{
    public string Name { get; set; } = string.Empty;

    public string ObjectType { get; set; } = string.Empty;

    public string UniqueId { get; set; } = string.Empty;

    public string CreatedBy { get; set; } = string.Empty;

    public string ModifiedBy { get; set; } = string.Empty;

    public DateTime? CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public int Revision { get; set; }

    public bool HasMissingReferences { get; set; }

    public string SynchronizationPolicy { get; set; } = string.Empty;

    public List<WorkspaceFolder> RootFolders { get; set; } = new();

    public List<WorkspaceUser> Users { get; set; } = new();

    public List<WorkspaceGroup> Groups { get; set; } = new();
}

