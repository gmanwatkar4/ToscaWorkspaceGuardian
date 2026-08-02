// <copyright file="IProcessRunner.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Common.Interfaces;

using System.Threading;
using ToscaWorkspaceGuardian.Common.Models;

/// <summary>

/// TODO: Describe IProcessRunner.

/// </summary>

public interface IProcessRunner
{
    Task<ProcessResult> ExecuteAsync(
        string fileName,
        string arguments,
        CancellationToken cancellationToken = default);
}

