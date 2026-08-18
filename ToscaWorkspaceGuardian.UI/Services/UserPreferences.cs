namespace ToscaWorkspaceGuardian.UI.Services;

public sealed class UserPreferences
{
    public string OutputFolder { get; set; } = string.Empty;

    public string SourceVersion { get; set; } = "2025.1";

    public string TargetVersion { get; set; } = "2026.1";
}
