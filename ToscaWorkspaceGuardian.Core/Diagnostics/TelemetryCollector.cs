<!--
// <copyright file="TelemetryCollector.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
-->
namespace ToscaWorkspaceGuardian.Core.Diagnostics;

using System;
using System.Collections.Concurrent;
using System.Threading;

/// <summary>

/// TODO: Describe TelemetryCollector.

/// </summary>

public class TelemetryCollector
{
    private long tcShellCallCount;
    private long tcShellTotalMilliseconds;
    private ConcurrentBag<int> exitCodes = new();

    private long crawlCount;
    private long crawlTotalMilliseconds;

    public void RecordTCShellCall(TimeSpan duration, int exitCode)
    {
        Interlocked.Increment(ref this.tcShellCallCount);
        Interlocked.Add(ref this.tcShellTotalMilliseconds, (long)duration.TotalMilliseconds);
        this.exitCodes.Add(exitCode);
    }

    public void RecordCrawl(TimeSpan duration)
    {
        Interlocked.Increment(ref this.crawlCount);
        Interlocked.Add(ref this.crawlTotalMilliseconds, (long)duration.TotalMilliseconds);
    }

    public string GetSummary()
    {
        double avgTcshell = 0;
        var calls = Interlocked.Read(ref this.tcShellCallCount);
        var totalMs = Interlocked.Read(ref this.tcShellTotalMilliseconds);
        if (calls > 0)
        {
            avgTcshell = totalMs / (double)calls;
        }

        double avgCrawl = 0;
        var crawls = Interlocked.Read(ref this.crawlCount);
        var crawlTotal = Interlocked.Read(ref this.crawlTotalMilliseconds);
        if (crawls > 0)
        {
            avgCrawl = crawlTotal / (double)crawls;
        }

        int failures = 0;
        foreach (var code in this.exitCodes)
        {
            if (code != 0)
            {
                failures++;
            }
        }

        return $"Telemetry: TCShellCalls={calls}, AvgTCShellMs={avgTcshell:0.0}, Failures={failures}, Crawls={crawls}, AvgCrawlMs={avgCrawl:0.0}";
    }
}

