namespace ToscaWorkspaceGuardian.Tests;

using ToscaWorkspaceGuardian.Core.Models;
using ToscaWorkspaceGuardian.Core.TQL;
using ToscaWorkspaceGuardian.Core.Upgrade;

public sealed class TqlUpgradeRiskScannerTests
{
    [Fact]
    public async Task ScanAsync_MapsOnlyMatchedObjectsToVersionedFindings()
    {
        var scanner = new TqlUpgradeRiskScanner(new UpgradeRiskRuleCatalog(), new StubQueryRunner());

        var result = await scanner.ScanAsync(new WorkspaceRequest
        {
            SourceVersion = "2025.1",
            TargetVersion = "2026.1",
        });

        var finding = Assert.Single(result.Findings);
        Assert.Equal("WG-2026-TQL-001", finding.RuleId);
        Assert.Equal("Array Iterator", finding.ObjectName);
        Assert.Empty(result.Diagnostics);
    }

    private sealed class StubQueryRunner : ITqlQueryRunner
    {
        public Task<TqlQueryResult> RunAsync(string query, WorkspaceRequest request, CancellationToken cancellationToken = default)
        {
            var document = new OutputDocument();
            if (query.Contains("Array To Iterate", StringComparison.Ordinal))
            {
                document.Objects.Add(new OutputObject { Name = "Array Iterator", ObjectType = "XModuleAttribute" });
            }

            return Task.FromResult(new TqlQueryResult
            {
                Success = true,
                Query = query,
                Document = document,
                Message = "OK",
            });
        }
    }
}
