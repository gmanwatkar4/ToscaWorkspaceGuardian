Title: Repo: repository-wide improvements (feature/repo-improvements)

Description:
This PR implements a set of repository-wide improvements focused on reliability, performance, and developer experience. Key changes:

- Enabled analyzers and added .editorconfig.
- Made ProcessRunner async, cancelable, and added Polly-based retry/timeout policies.
- Added telemetry (OpenTelemetry) and a lightweight TelemetryCollector.
- Implemented script caching, batch result caching, and cache eviction to reduce repeated I/O.
- Implemented bounded concurrency for WorkspaceCrawler and made batch size configurable via CrawlerOptions.
- Added NodeCacheService to skip traversal of unchanged nodes (incremental scanning).
- Optimized OutputParser (streaming + precompiled regex) and reduced LINQ allocations in hot paths.
- Added unit tests for ProcessRunner and TCShellExecutor and a synthetic benchmark project with CI job.
- Cleaned up unused empty folders and added README guidance files.

Notes:
- All changes are on branch feature/repo-improvements. The branch has been pushed to origin.
- CI workflow was updated to include analyzers and a synthetic benchmark job that uploads benchmark artifacts.

Suggested reviewers (replace with actual GitHub usernames):
- @your-reviewer-1
- @your-reviewer-2

How to create the PR locally (using GitHub CLI)
1. Install GitHub CLI (https://cli.github.com/) and authenticate (`gh auth login`).
2. Run:

   gh pr create --base main --head feature/repo-improvements --title "Repo: repository-wide improvements" --body-file PULL_REQUEST.md --draft

This opens a draft PR. To request reviewers:

   gh pr review --request "your-reviewer-1,your-reviewer-2" --repo gmanwatkar4/ToscaWorkspaceGuardian <pr-number>

Alternatively create the PR via web:

  https://github.com/gmanwatkar4/ToscaWorkspaceGuardian/pull/new/feature/repo-improvements
