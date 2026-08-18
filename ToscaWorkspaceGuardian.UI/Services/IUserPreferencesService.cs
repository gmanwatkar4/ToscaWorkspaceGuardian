namespace ToscaWorkspaceGuardian.UI.Services;

public interface IUserPreferencesService
{
    UserPreferences Load();

    void Save(UserPreferences preferences);
}
