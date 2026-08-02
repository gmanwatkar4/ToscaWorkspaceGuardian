// <copyright file="SnapshotBuilder.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Workspace;

using ToscaWorkspaceGuardian.Core.Business;
using ToscaWorkspaceGuardian.Core.Interfaces;
using ToscaWorkspaceGuardian.Core.Models;

/// <summary>

/// TODO: Describe SnapshotBuilder.

/// </summary>

public class SnapshotBuilder : ISnapshotBuilder
{
    public void AddDocument(
        WorkspaceSnapshot snapshot,
        OutputDocument document)
    {
        foreach (var obj in document.Objects)
        {
            var repositoryObject = new RepositoryObject
            {
                Name = obj.Name,
                ObjectType = obj.ObjectType,
            };

            //--------------------------------------------------
            // Properties
            //--------------------------------------------------
            foreach (var property in obj.Properties)
            {
                repositoryObject.Properties[property.Name] =
                    property.Value;
            }

            //--------------------------------------------------
            // Collections
            //--------------------------------------------------
            foreach (var collection in obj.Collections)
            {
                // create a copy of the collection values with a known capacity
                var valuesList = new List<string>(collection.Value.Count);
                for (int i = 0; i < collection.Value.Count; i++)
                {
                    valuesList.Add(collection.Value[i]);
                }

                repositoryObject.Collections[collection.Key] = valuesList;
            }

            //--------------------------------------------------
            // NodePath
            //--------------------------------------------------
            if (repositoryObject.Properties.TryGetValue(
            "NodePath",
            out var nodePath))
            {
                repositoryObject.NodePath = nodePath;

                int index = nodePath.LastIndexOf('/');

                if (index > 0)
                {
                    repositoryObject.ParentPath =
                        nodePath.Substring(0, index);
                }
                else
                {
                    repositoryObject.ParentPath = "/";
                }
            }

            snapshot.Objects.Add(repositoryObject);

            //------------------------------------------
            // NodePath Index
            //------------------------------------------
            if (!string.IsNullOrWhiteSpace(repositoryObject.NodePath))
            {
                snapshot.ByNodePath[repositoryObject.NodePath] =
                    repositoryObject;
            }

            //------------------------------------------
            // ObjectType Index
            //------------------------------------------
            if (!snapshot.ByObjectType.TryGetValue(
                    repositoryObject.ObjectType,
                    out var list))
            {
                list = new List<RepositoryObject>();

                snapshot.ByObjectType[repositoryObject.ObjectType] =
                    list;
            }

            list.Add(repositoryObject);
        }
    }
}

