namespace ToscaWorkspaceGuardian.Core.Reporting;

using System.Net;
using System.Text;
using ToscaWorkspaceGuardian.Core.Upgrade;

/// <summary>
/// Generates a portable HTML report for a completed upgrade-risk scan.
/// </summary>
public sealed class UpgradeRiskHtmlReportGenerator
{
    public async Task GenerateAsync(
        UpgradeRiskScanResult result,
        string sourceVersion,
        string targetVersion,
        string aiBriefing,
        string reportPath,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reportPath);
        Directory.CreateDirectory(Path.GetDirectoryName(reportPath)!);

        var html = new StringBuilder();
        html.Append("<!doctype html><html><head><meta charset=\"utf-8\"><title>Tosca Upgrade Risk Report</title>");
        html.Append("<style>body{font-family:Segoe UI,Arial,sans-serif;margin:32px;color:#1f2937}h1{color:#075985}h2{margin-top:30px}table{border-collapse:collapse;width:100%}th,td{border:1px solid #d1d5db;padding:9px;text-align:left;vertical-align:top}th{background:#e0f2fe}.warning{color:#b45309;font-weight:600}.advisory{color:#0369a1;font-weight:600}.briefing{white-space:pre-wrap;background:#f8fafc;border-left:4px solid #0ea5e9;padding:14px}</style></head><body>");
        html.Append("<h1>Tosca Upgrade Risk Report</h1>");
        html.Append($"<p><strong>Upgrade path:</strong> Tosca {Encode(sourceVersion)} → {Encode(targetVersion)}<br><strong>Generated:</strong> {DateTimeOffset.Now:dd MMM yyyy HH:mm}</p>");
        html.Append($"<p><strong>Findings:</strong> {result.Findings.Count}</p>");
        html.Append("<h2>AI Upgrade Copilot briefing</h2>");
        html.Append($"<div class=\"briefing\">{Encode(aiBriefing)}</div>");
        html.Append("<h2>Verified upgrade findings</h2><table><thead><tr><th>Rule</th><th>Severity</th><th>Affected object</th><th>Impact</th><th>Recommended action</th><th>TQL evidence</th></tr></thead><tbody>");

        foreach (var finding in result.Findings)
        {
            html.Append("<tr>");
            html.Append($"<td>{Encode(finding.RuleId)}</td>");
            html.Append($"<td class=\"{Encode(finding.Severity.ToLowerInvariant())}\">{Encode(finding.Severity)}</td>");
            html.Append($"<td>{Encode(finding.ObjectName)}<br><small>{Encode(finding.ObjectType)}</small></td>");
            html.Append($"<td>{Encode(finding.Description)}</td>");
            html.Append($"<td>{Encode(finding.RecommendedAction)}</td>");
            html.Append($"<td><code>{Encode(finding.TqlQuery)}</code></td>");
            html.Append("</tr>");
        }

        html.Append("</tbody></table>");
        if (result.Diagnostics.Count > 0)
        {
            html.Append("<h2>Rule diagnostics</h2><ul>");
            foreach (var diagnostic in result.Diagnostics)
            {
                html.Append($"<li>{Encode(diagnostic)}</li>");
            }

            html.Append("</ul>");
        }

        html.Append("</body></html>");
        await File.WriteAllTextAsync(reportPath, html.ToString(), cancellationToken);
    }

    private static string Encode(string? value) => WebUtility.HtmlEncode(value ?? string.Empty);
}
