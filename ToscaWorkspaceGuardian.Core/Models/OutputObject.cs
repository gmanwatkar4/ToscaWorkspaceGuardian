// <copyright file="OutputObject.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Models;

/// <summary>

/// TODO: Describe OutputObject.

/// </summary>

public class OutputObject
{
    public bool IsContainer =>
    this.ObjectType == "TCProject" ||
    this.ObjectType == "TCFolder" ||
    this.ObjectType == "OwnedFolder" ||
    this.ObjectType == "ExecutionEntryFolder";

    public string ObjectType { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string RawText { get; set; } = string.Empty;

    public List<OutputProperty> Properties { get; set; } = new();

    public Dictionary<string, List<string>> Collections { get; set; } = new();

    //--------------------------------------------------------
    // Helper Methods
    //--------------------------------------------------------
    public string? GetProperty(string propertyName)
    {
        if (propertyName == null)
        {
            return null;
        }

        for (int i = 0; i < this.Properties.Count; i++)
        {
            var p = this.Properties[i];
            if (p.Name == propertyName)
            {
                return p.Value;
            }
        }

        return null;
    }

    public IReadOnlyList<string> GetCollection(string collectionName)
    {
        if (this.Collections.TryGetValue(collectionName, out var values))
        {
            return values;
        }

        return Array.Empty<string>();
    }

    public bool HasCollection(string collectionName)
    {
        return this.Collections.ContainsKey(collectionName);
    }
}

