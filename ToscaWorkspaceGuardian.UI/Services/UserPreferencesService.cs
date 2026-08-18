namespace ToscaWorkspaceGuardian.UI.Services;

using System.IO;
using System.Text.Json;

public sealed class UserPreferencesService : IUserPreferencesService
{
    private readonly string settingsFile = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "ToscaWorkspaceGuardian",
        "preferences.json");

    public UserPreferences Load()
    {
        try
        {
            if (!File.Exists(this.settingsFile))
            {
                return new UserPreferences();
            }

            return JsonSerializer.Deserialize<UserPreferences>(File.ReadAllText(this.settingsFile))
                ?? new UserPreferences();
        }
        catch (JsonException)
        {
            return new UserPreferences();
        }
        catch (IOException)
        {
            return new UserPreferences();
        }
    }

    public void Save(UserPreferences preferences)
    {
        var directory = Path.GetDirectoryName(this.settingsFile)!;
        Directory.CreateDirectory(directory);
        var json = JsonSerializer.Serialize(preferences, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(this.settingsFile, json);
    }
}
