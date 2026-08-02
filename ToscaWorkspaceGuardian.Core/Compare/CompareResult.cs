// <copyright file="CompareResult.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Compare;

/// <summary>

/// TODO: Describe CompareResult.

/// </summary>

public class CompareResult
{
    public List<string> AddedObjects { get; set; } = new();

    public List<string> RemovedObjects { get; set; } = new();

    public List<PropertyChange> PropertyChanges { get; set; } = new();
}

