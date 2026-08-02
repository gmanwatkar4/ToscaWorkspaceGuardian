# Change log for feature/repo-improvements

This file summarizes the changes made on branch `feature/repo-improvements`.

## Changes

- Enabled Roslyn .NET analyzers globally (Microsoft.CodeAnalysis.NetAnalyzers).
- Added StyleCop.Analyzers for consistent code style checks.
- Made ProcessRunner cancellable and added convenience timeout and sync wrappers.
- Updated IProcessRunner interface to accept CancellationToken.
- Added unit tests: ProcessRunnerTest and TcshellExecutorTests.
- Added GitHub Actions workflow for CI (build + test) and upload of test artifacts.
