// <copyright file="PropertyChange.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Compare;

public class PropertyChange
{
    public string NodePath { get; set; } = string.Empty;

    public string Property { get; set; } = string.Empty;

    public string OldValue { get; set; } = string.Empty;

    public string NewValue { get; set; } = string.Empty;
}
