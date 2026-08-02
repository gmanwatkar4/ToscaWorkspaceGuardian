# Change log for feature/repo-improvements

This file summarizes the changes made on branch `feature/repo-improvements`.

## Changes

- Enabled Roslyn .NET analyzers globally (Microsoft.CodeAnalysis.NetAnalyzers).
- Added StyleCop.Analyzers for consistent code style checks.
- Made ProcessRunner cancellable and added convenience timeout and sync wrappers.
- Updated IProcessRunner interface to accept CancellationToken.
- Added unit tests: ProcessRunnerTest and TcshellExecutorTests.
- Added GitHub Actions workflow for CI (build + test) and upload of test artifacts.
 - Added TelemetryCollector and instrumentation for TCShell and crawler.
 - Implemented script caching with eviction and batch result caching to reduce repeated TCShell calls.
 - Implemented bounded concurrency and configurable crawler options.
 - Added NodeCacheService for incremental scanning: skip child traversal when node content unchanged.
 - Optimized OutputParser with precompiled regex and streaming line reads.
 - Reduced LINQ allocations in OutputObject.GetProperty and SnapshotBuilder.
 - Removed debug allocations from hot loops.
 - Added Polly-based retry and timeout policy for external process execution (configurable via env vars TWG_PROCESS_RETRIES and TWG_PROCESS_TIMEOUT_SECONDS).
 - Added OpenTelemetry tracing and metrics (console exporter) and instrumented TCShell and crawler with ActivitySource and metrics.
