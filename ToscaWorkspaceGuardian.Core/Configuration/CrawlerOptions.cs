// <copyright file="CrawlerOptions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace ToscaWorkspaceGuardian.Core.Configuration;

/// <summary>

/// TODO: Describe CrawlerOptions.

/// </summary>

public class CrawlerOptions
{
    // Maximum number of batches to process in parallel
    public int MaxDegreeOfParallelism { get; set; } = 4;

    // Maximum number of paths per batch
    public int BatchSize { get; set; } = 50;
}

