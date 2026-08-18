namespace ToscaWorkspaceGuardian.Core.Subset;

using System.IO.Compression;
using System.Text.Json;

/// <summary>
/// Reads Tosca .tsu exports without requiring a local Tosca installation.
/// </summary>
public sealed class ToscaSubsetAnalyzer : IToscaSubsetAnalyzer
{
    public async Task<SubsetAnalysisResult> AnalyzeAsync(
        string subsetPath,
        string sourceVersion,
        string targetVersion,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(subsetPath);

        await using var file = File.OpenRead(subsetPath);
        await using var gzip = new GZipStream(file, CompressionMode.Decompress);
        using var document = await JsonDocument.ParseAsync(gzip, cancellationToken: cancellationToken);

        var root = document.RootElement;
        var projectName = ReadString(root, "ProjectName") ?? Path.GetFileNameWithoutExtension(subsetPath);
        var entities = ReadEntities(root, cancellationToken);
        var entityCounts = entities
            .GroupBy(entity => entity.ObjectClass, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => group.Count(), StringComparer.OrdinalIgnoreCase);

        var findings = Is2025To2026Upgrade(sourceVersion, targetVersion)
            ? Find2026UpgradeConsiderations(entities)
            : new List<SubsetFinding>();

        return new SubsetAnalysisResult
        {
            ProjectName = projectName,
            EntityCount = entities.Count,
            EntityCounts = entityCounts,
            Findings = findings,
        };
    }

    private static List<SubsetEntity> ReadEntities(JsonElement root, CancellationToken cancellationToken)
    {
        var entities = new List<SubsetEntity>();
        if (!root.TryGetProperty("Entities", out var entityArray) || entityArray.ValueKind != JsonValueKind.Array)
        {
            return entities;
        }

        foreach (var entity in entityArray.EnumerateArray())
        {
            cancellationToken.ThrowIfCancellationRequested();
            var objectClass = ReadString(entity, "ObjectClass") ?? "Unknown";
            var attributes = ReadStringMap(entity, "Attributes");
            var associations = ReadStringMap(entity, "Assocs");
            entities.Add(new SubsetEntity(objectClass, attributes, associations));
        }

        return entities;
    }

    private static List<SubsetFinding> Find2026UpgradeConsiderations(IEnumerable<SubsetEntity> entities)
    {
        var findings = new List<SubsetFinding>();

        foreach (var attribute in entities.Where(entity => entity.ObjectClass.Equals("XModuleAttribute", StringComparison.OrdinalIgnoreCase)))
        {
            var name = attribute.Attributes.GetValueOrDefault("Name");
            if (!string.Equals(name, "Array To Iterate", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(name, "Target Buffer", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var module = attribute.Associations.GetValueOrDefault("Module");
            findings.Add(new SubsetFinding(
                "WG-2026-TSU-001",
                "Warning",
                string.IsNullOrWhiteSpace(module) ? name! : $"{module} / {name}",
                $"The legacy TBox Iterate Array attribute '{name}' is present in this subset.",
                "Update the module to use the Array ModuleAttribute introduced for Tosca 2026.1 and validate affected test cases."));
        }

        foreach (var configuration in entities.Where(entity => entity.ObjectClass.Equals("TCConfiguration", StringComparison.OrdinalIgnoreCase)))
        {
            var name = configuration.Attributes.GetValueOrDefault("Name");
            if (!string.Equals(name, "SeaLights", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            findings.Add(new SubsetFinding(
                "WG-2026-TSU-002",
                "Advisory",
                "SeaLights configuration",
                "A SeaLights configuration is included in this subset.",
                "Review SeaLights release notes and validate the integration after upgrading Tosca."));
        }

        var testDataClasses = entities.Count(entity => entity.ObjectClass.Equals("TestCaseTemplate", StringComparison.OrdinalIgnoreCase)
            || entity.ObjectClass.StartsWith("TD", StringComparison.OrdinalIgnoreCase));
        if (testDataClasses > 0)
        {
            findings.Add(new SubsetFinding(
                "WG-2026-TSU-003",
                "Advisory",
                $"{testDataClasses} TestCase Design entities",
                "The subset contains TestCase Design content.",
                "If your workflow uploads TestCase Design Classes to Tosca cloud, review the 2026.1.1 support limitations before rollout."));
        }

        return findings;
    }

    private static bool Is2025To2026Upgrade(string sourceVersion, string targetVersion) =>
        sourceVersion.StartsWith("2025", StringComparison.OrdinalIgnoreCase)
        && targetVersion.StartsWith("2026", StringComparison.OrdinalIgnoreCase);

    private static string? ReadString(JsonElement element, string propertyName)
    {
        return element.TryGetProperty(propertyName, out var property) && property.ValueKind == JsonValueKind.String
            ? property.GetString()
            : null;
    }

    private static Dictionary<string, string> ReadStringMap(JsonElement entity, string propertyName)
    {
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (!entity.TryGetProperty(propertyName, out var map) || map.ValueKind != JsonValueKind.Object)
        {
            return values;
        }

        foreach (var property in map.EnumerateObject())
        {
            values[property.Name] = property.Value.ValueKind == JsonValueKind.String
                ? property.Value.GetString() ?? string.Empty
                : property.Value.GetRawText();
        }

        return values;
    }

    private sealed record SubsetEntity(
        string ObjectClass,
        IReadOnlyDictionary<string, string> Attributes,
        IReadOnlyDictionary<string, string> Associations);
}
