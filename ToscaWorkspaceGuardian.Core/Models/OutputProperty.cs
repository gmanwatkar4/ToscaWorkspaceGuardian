// <copyright file="OutputProperty.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Models;

/// <summary>

/// TODO: Describe OutputProperty.

/// </summary>

public class OutputProperty
{
    public string Name { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public bool IsReadOnly { get; set; }
}

