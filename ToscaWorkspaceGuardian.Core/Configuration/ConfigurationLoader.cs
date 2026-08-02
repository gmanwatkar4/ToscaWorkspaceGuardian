using System.Text.Json;

namespace ToscaWorkspaceGuardian.Core.Configuration;

public class ConfigurationLoader
{
    public RuleConfiguration Load(string file)
    {
        if (!File.Exists(file))
        {
            var config = new RuleConfiguration();

            Directory.CreateDirectory(
                Path.GetDirectoryName(file)!);

            File.WriteAllText(
                file,
                JsonSerializer.Serialize(
                    config,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true
                    }));

            return config;
        }

        return JsonSerializer.Deserialize<RuleConfiguration>(
            File.ReadAllText(file))
            ?? new RuleConfiguration();
    }
}