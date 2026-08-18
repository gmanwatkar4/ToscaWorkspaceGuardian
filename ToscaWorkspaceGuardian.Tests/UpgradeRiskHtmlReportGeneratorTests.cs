namespace ToscaWorkspaceGuardian.Tests;

using ToscaWorkspaceGuardian.Core.Reporting;
using ToscaWorkspaceGuardian.Core.Upgrade;

public sealed class UpgradeRiskHtmlReportGeneratorTests
{
    [Fact]
    public async Task GenerateAsync_WritesFindingAndAiBriefing()
    {
        var reportPath = Path.Combine(Path.GetTempPath(), $"upgrade-report-{Guid.NewGuid():N}.html");
        try
        {
            var result = new UpgradeRiskScanResult
            {
                Findings = new[]
                {
                    new UpgradeRiskFinding("WG-1", "Warning", "Legacy attribute", "Module A", "XModuleAttribute", "Impact text", "Fix it", "=>SUBPARTS:XModuleAttribute"),
                },
            };

            await new UpgradeRiskHtmlReportGenerator().GenerateAsync(result, "2025.1", "2026.1", "AI briefing", reportPath);

            var html = await File.ReadAllTextAsync(reportPath);
            Assert.Contains("WG-1", html, StringComparison.Ordinal);
            Assert.Contains("AI briefing", html, StringComparison.Ordinal);
            Assert.Contains("Fix it", html, StringComparison.Ordinal);
        }
        finally
        {
            File.Delete(reportPath);
        }
    }
}
