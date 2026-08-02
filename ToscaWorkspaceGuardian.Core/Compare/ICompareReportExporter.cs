// <copyright file="ICompareReportExporter.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Compare;

/// <summary>

/// TODO: Describe ICompareReportExporter.

/// </summary>

public interface ICompareReportExporter
{
    Task ExportAsync(
        CompareResult result,
        string filePath,
        CancellationToken cancellationToken = default);
}

