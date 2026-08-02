Purpose
-------
RuleEngine contains rules and evaluators used to analyze workspace snapshots.

Removed folders
- Evaluators/
- Models/
- Rules/

Guidance
- Implement small, testable rule classes that expose metadata (Id, Severity, Description) and are discoverable via DI.
- Keep rule evaluation side-effect free and put I/O into services.

Suggested structure
- Rules/ (WG001_*.cs)
- Evaluators/
- Models/ (RuleResult, RuleMetadata)
