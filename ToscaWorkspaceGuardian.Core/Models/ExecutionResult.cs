// <copyright file="ExecutionResult.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Models;

/// <summary>

/// TODO: Describe ExecutionResult.

/// </summary>

public class ExecutionResult
{
    public bool Success { get; set; }

    public int ExitCode { get; set; }

    public string ScriptPath { get; set; } = string.Empty;

    public string OutputFile { get; set; } = string.Empty;

    public string ErrorFile { get; set; } = string.Empty;

    public string StandardOutput { get; set; } = string.Empty;

    public string StandardError { get; set; } = string.Empty;
}

