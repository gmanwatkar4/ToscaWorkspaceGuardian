using System;
using System.Collections.Concurrent;
using System.Threading;

namespace ToscaWorkspaceGuardian.Core.Diagnostics;

public class TelemetryCollector
{
    private long _tcShellCallCount;
    private long _tcShellTotalMilliseconds;
    private ConcurrentBag<int> _exitCodes = new();

    private long _crawlCount;
    private long _crawlTotalMilliseconds;

    public void RecordTCShellCall(TimeSpan duration, int exitCode)
    {
        Interlocked.Increment(ref _tcShellCallCount);
        Interlocked.Add(ref _tcShellTotalMilliseconds, (long)duration.TotalMilliseconds);
        _exitCodes.Add(exitCode);
    }

    public void RecordCrawl(TimeSpan duration)
    {
        Interlocked.Increment(ref _crawlCount);
        Interlocked.Add(ref _crawlTotalMilliseconds, (long)duration.TotalMilliseconds);
    }

    public string GetSummary()
    {
        double avgTcshell = 0;
        var calls = Interlocked.Read(ref _tcShellCallCount);
        var totalMs = Interlocked.Read(ref _tcShellTotalMilliseconds);
        if (calls > 0) avgTcshell = totalMs / (double)calls;

        double avgCrawl = 0;
        var crawls = Interlocked.Read(ref _crawlCount);
        var crawlTotal = Interlocked.Read(ref _crawlTotalMilliseconds);
        if (crawls > 0) avgCrawl = crawlTotal / (double)crawls;

        int failures = 0;
        foreach (var code in _exitCodes)
        {
            if (code != 0) failures++;
        }

        return $"Telemetry: TCShellCalls={calls}, AvgTCShellMs={avgTcshell:0.0}, Failures={failures}, Crawls={crawls}, AvgCrawlMs={avgCrawl:0.0}";
    }
}
