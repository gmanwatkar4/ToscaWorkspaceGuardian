namespace ToscaWorkspaceGuardian.Core.Reporting;

using System.Net;
using ToscaWorkspaceGuardian.Core.Upgrade;
using ToscaWorkspaceGuardian.Core.Workspace;

/// <summary>Produces an evidence-led 2025.1 to 2026.1 upgrade-readiness report without a target workspace.</summary>
public sealed class UpgradeComparisonHtmlReportGenerator
{
    private const string DocumentationUrl = "https://docs.tricentis.com/tosca-2026.1/en-us/content/upgrade/upgrade_changes.htm";
    private readonly IInventoryUpgradeRuleCatalog ruleCatalog;

    public UpgradeComparisonHtmlReportGenerator(IInventoryUpgradeRuleCatalog ruleCatalog)
    {
        this.ruleCatalog = ruleCatalog;
    }

    public async Task GenerateAsync(
        WorkspaceInventorySnapshot inventory,
        UpgradeRiskScanResult scan,
        string reportPath,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(inventory);
        Directory.CreateDirectory(Path.GetDirectoryName(reportPath)!);

        var warnings = scan.Findings.Count(finding => finding.Severity == "Warning");
        var advisories = scan.Findings.Count(finding => finding.Severity == "Advisory");
        var readinessScore = Math.Max(0, 100 - (warnings * 12) - (advisories * 4));
        var readinessLabel = warnings > 0 ? "Needs validation" : advisories > 0 ? "Ready with advisories" : "Ready for staging";
        var readinessClass = warnings > 0 ? "attention" : advisories > 0 ? "advisory" : "ready";
        var implementedRules = this.ruleCatalog.GetRules(inventory.SourceVersion, inventory.TargetVersion);
        var typeCounts = string.Join(string.Empty, inventory.Nodes
            .GroupBy(node => node.ObjectType)
            .OrderByDescending(group => group.Count())
            .Take(6)
            .Select(group => $"<li><span>{Encode(group.Key)}</span><b>{group.Count():N0}</b></li>"));
        var impactRows = BuildImpactRows(scan.Findings);
        var ruleLibraryRows = BuildRuleLibraryRows(implementedRules);

        var html = """
            <!doctype html><html><head><meta charset="utf-8"><title>Tosca Upgrade Readiness Report</title><style>
            :root{--tosca:#005ca9;--navy:#133f63;--ink:#223548;--muted:#66798a;--line:#d4e0ea;--panel:#f3f7fa;--warn:#a85a00;--warn-bg:#fff4df;--info:#0b659f;--info-bg:#e9f4fb;--good:#157a4b;--good-bg:#e8f7ef}*{box-sizing:border-box}body{margin:0;background:#eef3f6;color:var(--ink);font:15px "Segoe UI",Arial,sans-serif;line-height:1.48}.top{background:var(--tosca);color:#fff;padding:18px 0;border-bottom:4px solid #124772}.wrap{max-width:1180px;margin:auto;padding:0 30px}.brand{font-size:19px;font-weight:600}.brand small{display:block;letter-spacing:.08em;font-size:10px;font-weight:700;color:#d5eafa;margin-top:3px}.report{padding:28px 0 46px}.hero{background:#fff;border:1px solid var(--line);border-top:5px solid var(--tosca);padding:28px 30px;box-shadow:0 8px 24px #1a405214}.eyebrow{color:var(--tosca);font-size:11px;font-weight:700;letter-spacing:.09em}.hero h1{font-size:30px;color:var(--navy);margin:7px 0}.hero p{margin:8px 0;color:var(--muted)}.meta{font:12px Consolas,monospace;color:#536879;overflow-wrap:anywhere}.grid{display:grid;grid-template-columns:repeat(4,1fr);gap:14px;margin:18px 0}.card{background:#fff;border:1px solid var(--line);padding:18px}.metric{font-size:29px;font-weight:700;color:var(--tosca)}.metric-label{color:var(--muted);font-size:12px;margin-top:3px}.score{display:flex;align-items:center;gap:14px}.score-value{font-size:32px;font-weight:700}.badge{display:inline-block;font-size:11px;font-weight:700;text-transform:uppercase;padding:5px 9px;border-radius:2px}.badge.attention{background:var(--warn-bg);color:var(--warn)}.badge.advisory{background:var(--info-bg);color:var(--info)}.badge.ready{background:var(--good-bg);color:var(--good)}.section{background:#fff;border:1px solid var(--line);margin-top:18px;padding:24px}.section h2{color:var(--navy);font-size:19px;margin:0 0 5px}.section-intro{color:var(--muted);margin:0 0 16px}.comparison{display:grid;grid-template-columns:1fr 36px 1fr;gap:14px}.lane{border:1px solid var(--line);background:var(--panel);padding:18px}.lane h3{font-size:15px;color:var(--tosca);margin:0 0 10px}.arrow{align-self:center;text-align:center;font-size:25px;color:var(--tosca)}ul{margin:8px 0;padding-left:20px}.split{display:grid;grid-template-columns:1fr 1fr;gap:18px}.composition{list-style:none;padding:0}.composition li{display:flex;justify-content:space-between;border-bottom:1px solid var(--line);padding:7px 0}.composition li:last-child{border:0}.impact-table{width:100%;border-collapse:collapse}.impact-table th{font-size:11px;letter-spacing:.06em;text-transform:uppercase;color:#526a80;background:#edf4f8;text-align:left}.impact-table th,.impact-table td{padding:12px;border-bottom:1px solid var(--line);vertical-align:top}.impact-table td{font-size:13px}.severity{font-size:11px;font-weight:700;text-transform:uppercase}.severity.warning{color:var(--warn)}.severity.advisory{color:var(--info)}.assets{max-width:320px;overflow-wrap:anywhere;color:#506779}.rule-chip{position:relative;color:var(--tosca);cursor:help}.rule-chip:hover:after{content:attr(data-tooltip);position:absolute;z-index:2;left:0;top:22px;width:300px;padding:9px;background:var(--navy);color:#fff;font-weight:400;font-size:12px;line-height:1.35;box-shadow:0 5px 15px #0004}.rule-toggle{background:var(--tosca);color:#fff;border:0;padding:10px 14px;font:600 13px "Segoe UI",Arial;cursor:pointer;margin:4px 0 14px}.rule-library[hidden]{display:none}.runbook{display:grid;grid-template-columns:repeat(3,1fr);gap:14px}.step{background:var(--panel);border-left:4px solid var(--tosca);padding:14px}.step b{color:var(--navy)}.notice{background:#fff9ed;border:1px solid #f1d7a8;padding:14px;color:#6e4a10}.foot{color:var(--muted);font-size:12px;margin-top:20px}a{color:var(--tosca);font-weight:600}@media(max-width:780px){.wrap{padding:0 16px}.grid{grid-template-columns:repeat(2,1fr)}.comparison,.split{grid-template-columns:1fr}.arrow{transform:rotate(90deg)}.runbook{grid-template-columns:1fr}.impact-table{display:block;overflow-x:auto}}@media(max-width:460px){.grid{grid-template-columns:1fr}.hero{padding:22px 18px}.section{padding:18px}}
            </style></head><body><header class="top"><div class="wrap"><div class="brand">Tosca Upgrade Guide<small>READ-ONLY WORKSPACE ASSESSMENT</small></div></div></header><main class="wrap report">
            <section class="hero"><div class="eyebrow">UPGRADE READINESS REPORT</div><h1>Tosca @@SOURCE@@ &rarr; @@TARGET@@</h1><p>Evidence-based comparison of the scanned source workspace against documented destination-release changes. A 2026.1 workspace was not required and no workspace data was modified.</p><p class="meta">Source: @@WORKSPACE@@<br>Inventory: @@OBJECTS@@ nodes &middot; @@MODE@@ scan &middot; Generated @@GENERATED@@ UTC</p></section>
            <section class="grid"><article class="card"><div class="metric">@@OBJECTS@@</div><div class="metric-label">objects inspected through TCShell</div></article><article class="card"><div class="metric">@@RULES@@</div><div class="metric-label">documented rule matches</div></article><article class="card"><div class="metric">@@WARNINGS@@</div><div class="metric-label">items needing validation</div></article><article class="card"><div class="score"><div class="score-value">@@SCORE@@</div><div><span class="badge @@READINESS_CLASS@@">@@READINESS@@</span><div class="metric-label">readiness score / 100</div></div></div></article></section>
            <section class="section"><h2>Release comparison</h2><p class="section-intro">What the source release provides and what changes need attention in 2026.1.</p><div class="comparison"><article class="lane"><h3>2025.1 source baseline</h3><ul><li>SeaLights and Tosca Cloud transition capabilities.</li><li>Data Integrity and mobile automation enhancements.</li><li>2025.1 unique-ID logic is already present in this source release.</li><li>Standard subset is the module comparison baseline.</li></ul></article><div class="arrow">&rarr;</div><article class="lane"><h3>2026.1 destination impact</h3><ul><li>Legacy TBox Iterate Array attributes move to the new Array ModuleAttribute.</li><li>New branches cannot be created; snapshots replace future branch workflows.</li><li>PDF table and SAP NWBC modules may require rescan after upgrade.</li><li>DEX, OSV, SeaLights and cloud configuration locations may need operational review.</li></ul></article></div></section>
            <section class="section"><div class="split"><article><h2>Scanned workspace composition</h2><p class="section-intro">Top object types from the verified JSON inventory.</p><ul class="composition">@@TYPE_COUNTS@@</ul></article><article><h2>Assessment scope</h2><p class="section-intro">This prototype uses saved 2025.1 source evidence plus documented 2026.1 rules.</p><div class="notice"><b>Interpretation:</b> a rule match means “validate before production,” not that a test is already broken. A no-match result is not a guarantee of zero upgrade risk.</div><p><a href="@@DOC_URL@@">Open official 2026.1 changes and deprecations</a></p></article></div></section>
            <section class="section"><h2>Evidence-based impact register</h2><p class="section-intro">Consolidated by rule to avoid duplicate noise while retaining every affected inventory path. Hover a Rule ID for a short description.</p><table class="impact-table"><thead><tr><th>Rule</th><th>Severity</th><th>Impact and required action</th><th>Source-workspace evidence</th></tr></thead><tbody>@@IMPACT_ROWS@@</tbody></table></section>
            <section class="section"><h2>Implemented rule library</h2><p class="section-intro">The exact JSON-backed rules currently enabled for this upgrade path.</p><button class="rule-toggle" type="button" onclick="var x=document.getElementById('rule-library');x.hidden=!x.hidden;this.textContent=x.hidden?'View implemented rules':'Hide implemented rules';">View implemented rules</button><div id="rule-library" class="rule-library" hidden><table class="impact-table"><thead><tr><th>Rule</th><th>Match definition</th><th>Description</th><th>Recommended action</th></tr></thead><tbody>@@RULE_LIBRARY_ROWS@@</tbody></table></div></section>
            <section class="section"><h2>Recommended upgrade runbook</h2><div class="runbook"><article class="step"><b>1. Stage</b><br>Copy to a dedicated test repository and install the intended 2026.1 patch.</article><article class="step"><b>2. Remediate</b><br>Update each flagged module or configuration, then check in only from the upgraded estate.</article><article class="step"><b>3. Validate</b><br>Run impacted test cases and optional recovery-based execution-health checks before production downtime.</article></div></section>
            <p class="foot">Rules are derived from official Tricentis release and upgrade documentation. Credentials are excluded from this report and inventory.</p></main></body></html>
            """
            .Replace("@@SOURCE@@", Encode(inventory.SourceVersion), StringComparison.Ordinal)
            .Replace("@@TARGET@@", Encode(inventory.TargetVersion), StringComparison.Ordinal)
            .Replace("@@WORKSPACE@@", Encode(inventory.WorkspacePath), StringComparison.Ordinal)
            .Replace("@@OBJECTS@@", inventory.ObjectCount.ToString("N0"), StringComparison.Ordinal)
            .Replace("@@MODE@@", Encode(inventory.AnalysisMode), StringComparison.Ordinal)
            .Replace("@@GENERATED@@", Encode(inventory.GeneratedAtUtc.ToString("yyyy-MM-dd HH:mm:ss")), StringComparison.Ordinal)
            .Replace("@@RULES@@", scan.Findings.Count.ToString("N0"), StringComparison.Ordinal)
            .Replace("@@WARNINGS@@", warnings.ToString("N0"), StringComparison.Ordinal)
            .Replace("@@SCORE@@", readinessScore.ToString(), StringComparison.Ordinal)
            .Replace("@@READINESS@@", readinessLabel, StringComparison.Ordinal)
            .Replace("@@READINESS_CLASS@@", readinessClass, StringComparison.Ordinal)
            .Replace("@@TYPE_COUNTS@@", typeCounts, StringComparison.Ordinal)
            .Replace("@@DOC_URL@@", DocumentationUrl, StringComparison.Ordinal)
            .Replace("@@IMPACT_ROWS@@", impactRows, StringComparison.Ordinal)
            .Replace("@@RULE_LIBRARY_ROWS@@", ruleLibraryRows, StringComparison.Ordinal);

        await File.WriteAllTextAsync(reportPath, html, cancellationToken);
    }

    private static string BuildImpactRows(IReadOnlyList<UpgradeRiskFinding> findings)
    {
        if (findings.Count == 0)
        {
            return "<tr><td colspan=\"4\">No currently bundled rule matched this inventory. Review the scope and proceed with staging validation.</td></tr>";
        }

        return string.Join(string.Empty, findings
            .GroupBy(finding => new { finding.RuleId, finding.Severity, finding.Title, finding.Description, finding.RecommendedAction })
            .Select(group =>
            {
                var paths = group.Select(finding => finding.TqlQuery)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();
                var distinctArtifacts = group.Select(finding => finding.ObjectName)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .Count();
                var sharedScope = FindSharedInventoryScope(paths);
                var scopeSummary = string.IsNullOrWhiteSpace(sharedScope)
                    ? $"{distinctArtifacts} affected artifact(s) · {paths.Count} reference(s)"
                    : $"1 affected component · {distinctArtifacts} unique artifact(s) · {paths.Count} reference(s)<br><span class=\"evidence-scope\">Component: {Encode(sharedScope)}</span>";
                var evidence = string.Join("<br>", paths.Select(path => $"<span class=\"evidence-item\">{Encode(path)}</span>"));
                return $"<tr><td><span class=\"rule-chip\" data-tooltip=\"{Encode(group.Key.Description)}\"><b>{Encode(group.Key.RuleId)}</b></span><br>{Encode(group.Key.Title)}</td><td><span class=\"severity {Encode(group.Key.Severity.ToLowerInvariant())}\">{Encode(group.Key.Severity)}</span></td><td>{Encode(group.Key.Description)}<br><br><b>Action:</b> {Encode(group.Key.RecommendedAction)}</td><td class=\"assets\"><b>{scopeSummary}</b><details class=\"evidence-details\"><summary>Show {paths.Count} scanned reference(s)</summary>{evidence}</details></td></tr>";
            }));
    }

    private static string FindSharedInventoryScope(IReadOnlyList<string> evidence)
    {
        var paths = evidence
            .Select(item => item.StartsWith("Inventory node: ", StringComparison.OrdinalIgnoreCase)
                ? item["Inventory node: ".Length..].Split(" | Documentation: ", 2, StringSplitOptions.None)[0]
                : string.Empty)
            .Where(path => !string.IsNullOrWhiteSpace(path))
            .Select(path => path.Split('/', StringSplitOptions.RemoveEmptyEntries))
            .ToList();
        if (paths.Count < 2)
        {
            return string.Empty;
        }

        var commonLength = paths.Min(path => path.Length);
        for (var index = 0; index < commonLength; index++)
        {
            if (paths.Any(path => !string.Equals(path[index], paths[0][index], StringComparison.OrdinalIgnoreCase)))
            {
                commonLength = index;
                break;
            }
        }

        return commonLength == 0 ? string.Empty : "/" + string.Join('/', paths[0].Take(commonLength));
    }

    private static string BuildRuleLibraryRows(IReadOnlyList<InventoryUpgradeRuleDefinition> rules) =>
        string.Join(string.Empty, rules.Select(rule =>
        {
            var matchDefinition = string.Equals(rule.MatchMode, "ExactName", StringComparison.Ordinal)
                ? $"{Encode(rule.ObjectType)} named {Encode(string.Join(" or ", rule.Names))}"
                : $"Any inventory name or property containing {Encode(rule.Contains)}";
            return $"<tr><td><span class=\"rule-chip\" data-tooltip=\"{Encode(rule.Description)}\"><b>{Encode(rule.RuleId)}</b></span><br>{Encode(rule.Title)}<br><a href=\"{Encode(rule.DocumentationUrl)}\">Documentation</a></td><td>{matchDefinition}</td><td>{Encode(rule.Description)}</td><td>{Encode(rule.RecommendedAction)}</td></tr>";
        }));

    private static string Encode(string? value) => WebUtility.HtmlEncode(value ?? string.Empty);
}
