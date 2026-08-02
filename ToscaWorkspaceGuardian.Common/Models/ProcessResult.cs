// <copyright file="ProcessResult.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Common.Models;

/// <summary>

/// TODO: Describe ProcessResult.

/// </summary>

public class ProcessResult
{
    public int ExitCode { get; set; }

    public string StandardOutput { get; set; } = string.Empty;

    public string StandardError { get; set; } = string.Empty;

    public bool Success => this.ExitCode == 0;
}

