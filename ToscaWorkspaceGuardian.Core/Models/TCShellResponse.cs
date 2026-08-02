// <copyright file="TCShellResponse.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Models;

/// <summary>

/// TODO: Describe TCShellResponse.

/// </summary>

public class TCShellResponse
{
    public bool Success { get; set; }

    public int ExitCode { get; set; }

    public string Output { get; set; } = string.Empty;

    public string Error { get; set; } = string.Empty;
}

