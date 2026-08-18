namespace ToscaWorkspaceGuardian.Tests;

using System.IO.Compression;
using System.Text;
using ToscaWorkspaceGuardian.Core.Subset;

public sealed class ToscaSubsetAnalyzerTests
{
    [Fact]
    public async Task AnalyzeAsync_FindsDocumented2026MigrationConsiderations()
    {
        var subsetPath = Path.Combine(Path.GetTempPath(), $"guardian-{Guid.NewGuid():N}.tsu");
        const string content = """
            {
              "ProjectName": "Sample subset",
              "Entities": [
                { "ObjectClass": "XModuleAttribute", "Attributes": { "Name": "Array To Iterate" }, "Assocs": { "Module": "TBox Iterate Array" } },
                { "ObjectClass": "TCConfiguration", "Attributes": { "Name": "SeaLights" }, "Assocs": {} },
                { "ObjectClass": "TestCaseTemplate", "Attributes": {}, "Assocs": {} }
              ]
            }
            """;

        try
        {
            await using (var output = File.Create(subsetPath))
            await using (var gzip = new GZipStream(output, CompressionMode.Compress))
            {
                var bytes = Encoding.UTF8.GetBytes(content);
                await gzip.WriteAsync(bytes);
            }

            var result = await new ToscaSubsetAnalyzer().AnalyzeAsync(subsetPath, "2025.1", "2026.1");

            Assert.Equal("Sample subset", result.ProjectName);
            Assert.Equal(3, result.EntityCount);
            Assert.Contains(result.Findings, finding => finding.RuleId == "WG-2026-TSU-001");
            Assert.Contains(result.Findings, finding => finding.RuleId == "WG-2026-TSU-002");
            Assert.Contains(result.Findings, finding => finding.RuleId == "WG-2026-TSU-003");
        }
        finally
        {
            File.Delete(subsetPath);
        }
    }
}
