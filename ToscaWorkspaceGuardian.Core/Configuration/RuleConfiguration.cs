// <copyright file="RuleConfiguration.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Configuration;

public class RuleConfiguration
{
    public bool WG001 { get; set; } = true;

    public bool WG002 { get; set; } = true;

    public bool WG003 { get; set; } = true;

    public bool WG004 { get; set; } = true;

    public string SeverityThreshold { get; set; } = "Info";
}
