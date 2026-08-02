// <copyright file="ToscaInstallation.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Common.Models;

public class ToscaInstallation
{
    public bool IsInstalled { get; set; }

    public string InstallationPath { get; set; } = string.Empty;

    public string TCShellPath { get; set; } = string.Empty;

    public string ToscaCommanderPath { get; set; } = string.Empty;

    public string TBoxPath { get; set; } = string.Empty;

    public string Version { get; set; } = string.Empty;
}
