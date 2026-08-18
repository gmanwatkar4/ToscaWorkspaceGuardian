namespace ToscaWorkspaceGuardian.Tests;

using Microsoft.Data.Sqlite;
using ToscaWorkspaceGuardian.Core.Repository;

public sealed class ToscaRepositoryDatabaseAnalyzerTests
{
    [Fact]
    public async Task AnalyzeAsync_ReadsObjectInventoryAndLeavesDatabaseUnchanged()
    {
        var databasePath = Path.Combine(Path.GetTempPath(), $"guardian-{Guid.NewGuid():N}.db");

        try
        {
            await CreateRepositoryAsync(databasePath);
            var lastWriteTime = File.GetLastWriteTimeUtc(databasePath);

            var result = await new ToscaRepositoryDatabaseAnalyzer().AnalyzeAsync(databasePath, "2025.1", "2026.1");

            Assert.Equal(3, result.TotalObjects);
            Assert.Equal(3, result.ObjectTypes);
            Assert.Contains(result.TypeCounts, type => type.ObjectType == "XModuleAttribute" && type.Count == 1);
            Assert.Contains(result.Findings, finding => finding.RuleId == "WG-2026-DB-001");
            Assert.Equal(lastWriteTime, File.GetLastWriteTimeUtc(databasePath));
        }
        finally
        {
            File.Delete(databasePath);
        }
    }

    private static async Task CreateRepositoryAsync(string databasePath)
    {
        await using var connection = new SqliteConnection($"Data Source={databasePath};Pooling=False");
        await connection.OpenAsync();
        await using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE tcType (id INTEGER PRIMARY KEY, name TEXT NOT NULL);
            CREATE TABLE tcObject (surrogate TEXT PRIMARY KEY, typeId INTEGER NOT NULL);
            INSERT INTO tcType VALUES (1, 'XModuleAttribute'), (2, 'TestCase'), (3, 'TDClass');
            INSERT INTO tcObject VALUES ('one', 1), ('two', 2), ('three', 3);
            """;
        await command.ExecuteNonQueryAsync();
    }
}
