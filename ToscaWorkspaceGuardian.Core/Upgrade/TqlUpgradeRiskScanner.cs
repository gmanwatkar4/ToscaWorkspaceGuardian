namespace ToscaWorkspaceGuardian.Core.Upgrade;

using ToscaWorkspaceGuardian.Core.Models;
using ToscaWorkspaceGuardian.Core.TQL;

/// <summary>
/// Executes approved versioned upgrade rules against a live Tosca workspace.
/// </summary>
public sealed class TqlUpgradeRiskScanner : IUpgradeRiskScanner
{
    private readonly UpgradeRiskRuleCatalog ruleCatalog;
    private readonly ITqlQueryRunner queryRunner;

    public TqlUpgradeRiskScanner(UpgradeRiskRuleCatalog ruleCatalog, ITqlQueryRunner queryRunner)
    {
        this.ruleCatalog = ruleCatalog;
        this.queryRunner = queryRunner;
    }

    public async Task<UpgradeRiskScanResult> ScanAsync(WorkspaceRequest request, CancellationToken cancellationToken = default)
    {
        var findings = new List<UpgradeRiskFinding>();
        var diagnostics = new List<string>();

        foreach (var rule in this.ruleCatalog.GetRules(request.SourceVersion, request.TargetVersion))
        {
            var result = await this.queryRunner.RunAsync(rule.TqlQuery, request, cancellationToken);
            if (!result.Success)
            {
                diagnostics.Add($"{rule.RuleId}: {result.Message}");
                continue;
            }

            foreach (var item in result.Document.Objects)
            {
                findings.Add(new UpgradeRiskFinding(
                    rule.RuleId,
                    rule.Severity,
                    rule.Title,
                    item.Name,
                    item.ObjectType,
                    rule.Description,
                    rule.RecommendedAction,
                    rule.TqlQuery));
            }
        }

        return new UpgradeRiskScanResult
        {
            Findings = findings,
            Diagnostics = diagnostics,
        };
    }
}
