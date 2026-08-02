// <copyright file="WorkspaceHtmlReportGenerator.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Reporting;

using System.Net;
using System.Text;
using ToscaWorkspaceGuardian.Core.Business;

public class WorkspaceHtmlReportGenerator
{
    public async Task GenerateAsync(
        RepositoryStatistics statistics,
        IReadOnlyCollection<HealthIssue> issues,
        string outputFile)
    {
        var html = new StringBuilder();

        html.AppendLine("<!DOCTYPE html>");
        html.AppendLine("<html>");
        html.AppendLine("<head>");
        html.AppendLine("<meta charset=\"utf-8\" />");
        html.AppendLine("<title>Workspace Guardian Report</title>");

        html.AppendLine("<style>");
        html.AppendLine("body{font-family:'Segoe UI',Arial;background:#f3f5f7;margin:40px;color:#333;}");
        html.AppendLine("h1{color:#1976d2;margin-bottom:5px;}");
        html.AppendLine("h2{margin-top:0;color:#444;}");
        html.AppendLine(".subtitle{color:#666;margin-bottom:30px;}");
        html.AppendLine(".cards{display:flex;gap:20px;flex-wrap:wrap;margin-bottom:30px;}");
        html.AppendLine(".card{flex:1;min-width:180px;background:white;padding:20px;border-radius:10px;box-shadow:0 2px 8px rgba(0,0,0,.15);text-align:center;}");
        html.AppendLine(".card h3{margin:0;color:#666;font-size:16px;}");
        html.AppendLine(".card h1{margin:12px 0 0;font-size:42px;color:#1976d2;}");
        html.AppendLine(".section{background:white;padding:20px;margin-bottom:25px;border-radius:10px;box-shadow:0 2px 8px rgba(0,0,0,.15);}");
        html.AppendLine("table{border-collapse:collapse;width:100%;}");
        html.AppendLine("th,td{border:1px solid #ddd;padding:10px;text-align:left;}");
        html.AppendLine("th{background:#1976d2;color:white;}");
        html.AppendLine("tr:nth-child(even){background:#f8f8f8;}");
        html.AppendLine("</style>");

        html.AppendLine("</head>");
        html.AppendLine("<body>");

        //---------------------------------------
        // Header
        //---------------------------------------
        html.AppendLine("<h1>Workspace Guardian Report</h1>");
        html.AppendLine($"<div class='subtitle'>Generated : {statistics.GeneratedOn:dd-MMM-yyyy HH:mm:ss}</div>");

        //---------------------------------------
        // Dashboard Cards
        //---------------------------------------
        html.AppendLine("<div class='cards'>");

        AddCard("Objects", statistics.TotalObjects.ToString());
        AddCard("Health Issues", statistics.HealthIssueCount.ToString());
        AddCard("Health Score", statistics.HealthScore + "%");
        AddCard("Modules", statistics.ModuleCount.ToString());

        html.AppendLine("</div>");

        //---------------------------------------
        // Repository Statistics
        //---------------------------------------
        html.AppendLine("<div class='section'>");
        html.AppendLine("<h2>Repository Statistics</h2>");

        html.AppendLine("<table>");
        html.AppendLine("<tr><th>Metric</th><th>Value</th></tr>");

        AddStat("Folders", statistics.FolderCount);
        AddStat("Modules", statistics.ModuleCount);
        AddStat("Test Cases", statistics.TestCaseCount);
        AddStat("Execution Lists", statistics.ExecutionListCount);
        AddStat("Requirements", statistics.RequirementCount);
        AddStat("Users", statistics.UserCount);
        AddStat("Groups", statistics.GroupCount);

        html.AppendLine("</table>");
        html.AppendLine("</div>");

        //---------------------------------------
        // Rule Summary
        //---------------------------------------
        html.AppendLine("<div class='section'>");
        html.AppendLine("<h2>Rule Summary</h2>");

        html.AppendLine("<table>");
        html.AppendLine("<tr><th>Rule</th><th>Issue Count</th></tr>");

        foreach (var rule in statistics.RuleCounts.OrderBy(r => r.Key))
        {
            html.AppendLine("<tr>");
            html.AppendLine($"<td>{rule.Key}</td>");
            html.AppendLine($"<td>{rule.Value}</td>");
            html.AppendLine("</tr>");
        }

        html.AppendLine("</table>");
        html.AppendLine("</div>");

        //---------------------------------------
        // Health Issues
        //---------------------------------------
        html.AppendLine("<div class='section'>");
        html.AppendLine("<h2>Health Issues</h2>");

        html.AppendLine("<table>");

        html.AppendLine("<tr>");
        html.AppendLine("<th>Rule</th>");
        html.AppendLine("<th>Severity</th>");
        html.AppendLine("<th>Object</th>");
        html.AppendLine("<th>Node Path</th>");
        html.AppendLine("<th>Description</th>");
        html.AppendLine("</tr>");

        foreach (var issue in issues)
        {
            string color = issue.Severity switch
            {
                "High" => "#d32f2f",
                "Medium" => "#f57c00",
                "Low" => "#1976d2",
                _ => "#555555",
            };

            html.AppendLine("<tr>");
            html.AppendLine($"<td>{issue.RuleId}</td>");
            html.AppendLine($"<td style='font-weight:bold;color:{color}'>{issue.Severity}</td>");
            html.AppendLine($"<td>{WebUtility.HtmlEncode(issue.ObjectName)}</td>");
            html.AppendLine($"<td>{WebUtility.HtmlEncode(issue.NodePath)}</td>");
            html.AppendLine($"<td>{WebUtility.HtmlEncode(issue.Description)}</td>");
            html.AppendLine("</tr>");
        }

        html.AppendLine("</table>");
        html.AppendLine("</div>");

        html.AppendLine("</body>");
        html.AppendLine("</html>");

        Directory.CreateDirectory(Path.GetDirectoryName(outputFile)!);

        await File.WriteAllTextAsync(
            outputFile,
            html.ToString());

        //---------------------------------------------------
        // Local helper methods
        //---------------------------------------------------
        void AddCard(string title, string value)
        {
            html.AppendLine("<div class='card'>");
            html.AppendLine($"<h3>{title}</h3>");
            html.AppendLine($"<h1>{value}</h1>");
            html.AppendLine("</div>");
        }

        void AddStat(string name, int value)
        {
            html.AppendLine("<tr>");
            html.AppendLine($"<td>{name}</td>");
            html.AppendLine($"<td>{value}</td>");
            html.AppendLine("</tr>");
        }
    }
}
