Purpose
-------
Common contains shared models, utilities and services used across the solution.

Removed folders
- Constants/
- Enums/
- Extensions/
- Helpers/
- Logging/
- Models/Analysis/
- Models/Reporting/
- Models/Versioning/
- Models/Workspace/

Guidance
- Keep shared concerns minimal and well-tested. Prefer immutable models (records) where appropriate.
- Add extension methods and helpers only when they are broadly useful; keep logging and configuration centralized.

Suggested structure
- Models/
- Extensions/
- Utilities/
- Interfaces/

Notes
- Recreate folders only when adding content. Avoid committing empty folders.
