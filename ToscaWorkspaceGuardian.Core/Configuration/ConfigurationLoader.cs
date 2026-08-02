// <copyright file="ConfigurationLoader.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Configuration;

using System.Text.Json;

/// <summary>

/// TODO: Describe ConfigurationLoader.

/// </summary>

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
                        WriteIndented = true,
                    }));

            return config;
        }

        return JsonSerializer.Deserialize<RuleConfiguration>(
            File.ReadAllText(file))
            ?? new RuleConfiguration();
    }
}

