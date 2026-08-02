// <copyright file="IHealthReportExporter.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Interfaces;

using ToscaWorkspaceGuardian.Core.Business;

/// <summary>

/// TODO: Describe IHealthReportExporter.

/// </summary>

public interface IHealthReportExporter
{
    Task ExportAsync(
        IEnumerable<HealthIssue> issues,
        string outputFile,
        CancellationToken cancellationToken = default);
}

