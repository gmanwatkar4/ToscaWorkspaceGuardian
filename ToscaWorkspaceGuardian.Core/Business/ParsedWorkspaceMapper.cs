// <copyright file="ParsedWorkspaceMapper.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Business;

using ToscaWorkspaceGuardian.Core.Interfaces;
using ToscaWorkspaceGuardian.Core.Models;

/// <summary>

/// TODO: Describe ParsedWorkspaceMapper.

/// </summary>

public class ParsedWorkspaceMapper : IParsedWorkspaceMapper
{
    public ParsedWorkspace Map(OutputDocument document)
    {
        var workspace = new ParsedWorkspace();

        if (document.Objects.Count == 0)
        {
            return workspace;
        }

        var project = document.Objects[0];

        workspace.Name = project.Name;
        workspace.ObjectType = project.ObjectType;

        foreach (var property in project.Properties)
        {
            switch (property.Name)
            {
                case "UniqueId":
                    workspace.UniqueId = property.Value;
                    break;

                case "CreatedBy":
                    workspace.CreatedBy = property.Value;
                    break;

                case "ModifiedBy":
                    workspace.ModifiedBy = property.Value;
                    break;

                case "CreatedAt":
                    if (DateTime.TryParse(property.Value, out var created))
                    {
                        workspace.CreatedAt = created;
                    }

                    break;

                case "ModifiedAt":
                    if (DateTime.TryParse(property.Value, out var modified))
                    {
                        workspace.ModifiedAt = modified;
                    }

                    break;

                case "Revision":
                    if (int.TryParse(property.Value, out var revision))
                    {
                        workspace.Revision = revision;
                    }

                    break;

                case "HasMissingReferences":
                    if (bool.TryParse(property.Value, out var missing))
                    {
                        workspace.HasMissingReferences = missing;
                    }

                    break;

                case "SynchronizationPolicy":
                    workspace.SynchronizationPolicy = property.Value;
                    break;
            }
        }

        if (project.Collections.TryGetValue("Items", out var folders))
        {
            foreach (var folder in folders)
            {
                workspace.RootFolders.Add(
                    new WorkspaceFolder
                    {
                        Name = folder,
                    });
            }
        }

        if (project.Collections.TryGetValue("Users", out var users))
        {
            foreach (var user in users)
            {
                workspace.Users.Add(
                    new WorkspaceUser
                    {
                        Name = user,
                    });
            }
        }

        if (project.Collections.TryGetValue("Groups", out var groups))
        {
            foreach (var group in groups)
            {
                workspace.Groups.Add(
                    new WorkspaceGroup
                    {
                        Name = group,
                    });
            }
        }

        return workspace;
    }
}

