namespace ToscaWorkspaceGuardian.Core.Repository;

using Microsoft.Data.Sqlite;
using ToscaWorkspaceGuardian.Core.Subset;

/// <summary>
/// Reads supported Tosca local repository databases without changing their contents.
/// </summary>
public sealed class ToscaRepositoryDatabaseAnalyzer : IToscaRepositoryDatabaseAnalyzer
{
    public async Task<RepositoryDatabaseAnalysisResult> AnalyzeAsync(
        string databasePath,
        string sourceVersion,
        string targetVersion,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databasePath);

        var connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = databasePath,
            Mode = SqliteOpenMode.ReadOnly,
            Pooling = false,
        }.ToString();

        await using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        var totalObjects = await ExecuteCountAsync(connection, "SELECT COUNT(*) FROM tcObject", cancellationToken);
        var objectTypes = await ExecuteCountAsync(connection, "SELECT COUNT(*) FROM tcType", cancellationToken);
        var typeCounts = await ReadTypeCountsAsync(connection, cancellationToken);

        return new RepositoryDatabaseAnalysisResult
        {
            DatabasePath = databasePath,
            TotalObjects = totalObjects,
            ObjectTypes = objectTypes,
            TypeCounts = typeCounts,
            Findings = FindUpgradeConsiderations(typeCounts, sourceVersion, targetVersion),
        };
    }

    private static async Task<int> ExecuteCountAsync(SqliteConnection connection, string commandText, CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = commandText;
        return Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken));
    }

    private static async Task<IReadOnlyList<RepositoryTypeCount>> ReadTypeCountsAsync(
        SqliteConnection connection,
        CancellationToken cancellationToken)
    {
        const string commandText = """
            SELECT type.name, COUNT(object.surrogate)
            FROM tcType AS type
            LEFT JOIN tcObject AS object ON object.typeId = type.id
            GROUP BY type.id, type.name
            HAVING COUNT(object.surrogate) > 0
            ORDER BY COUNT(object.surrogate) DESC, type.name
            """;

        var results = new List<RepositoryTypeCount>();
        await using var command = connection.CreateCommand();
        command.CommandText = commandText;
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(new RepositoryTypeCount(reader.GetString(0), reader.GetInt32(1)));
        }

        return results;
    }

    private static IReadOnlyList<SubsetFinding> FindUpgradeConsiderations(
        IReadOnlyList<RepositoryTypeCount> typeCounts,
        string sourceVersion,
        string targetVersion)
    {
        if (!sourceVersion.StartsWith("2025", StringComparison.OrdinalIgnoreCase)
            || !targetVersion.StartsWith("2026", StringComparison.OrdinalIgnoreCase))
        {
            return Array.Empty<SubsetFinding>();
        }

        var findings = new List<SubsetFinding>();
        var moduleAttributes = typeCounts.FirstOrDefault(type => type.ObjectType.Equals("XModuleAttribute", StringComparison.OrdinalIgnoreCase));
        if (moduleAttributes is not null)
        {
            findings.Add(new SubsetFinding(
                "WG-2026-DB-001",
                "Advisory",
                $"{moduleAttributes.Count:N0} module attributes",
                "The database contains module attributes, whose individual names are stored in Tosca's object payload format.",
                "Export the relevant Modules as a .tsu and run Subset Upgrade Analyzer to find legacy TBox Iterate Array attributes."));
        }

        var testCaseDesignCount = typeCounts
            .Where(type => type.ObjectType.Equals("TestCaseTemplate", StringComparison.OrdinalIgnoreCase)
                || type.ObjectType.StartsWith("TD", StringComparison.OrdinalIgnoreCase))
            .Sum(type => type.Count);
        if (testCaseDesignCount > 0)
        {
            findings.Add(new SubsetFinding(
                "WG-2026-DB-002",
                "Advisory",
                $"{testCaseDesignCount:N0} TestCase Design objects",
                "TestCase Design content is present in the local repository.",
                "Review 2026.1 cloud-upload limitations for TestCase Design Classes if your workflow uses Tosca cloud."));
        }

        return findings;
    }
}
