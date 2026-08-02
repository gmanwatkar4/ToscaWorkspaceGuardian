Purpose
-------
This project implements the WPF UI for ToscaWorkspaceGuardian. During cleanup, empty resource folders were removed to keep the repository tidy.

Removed folders
- Assets/
- Converters/
- Controls/
- Models/
- Resources/
- Services/
- Themes/

If you need to add UI resources or code, create the appropriate folder and add csproj entries only if necessary (SDK style projects pick up files automatically). Keep view models under a ViewModels folder and views under Views.

Suggested structure
- Views/
- ViewModels/
- Controls/
- Resources/

Notes
- This README is informational. Recreate folders if you add content; avoid committing empty folders.
