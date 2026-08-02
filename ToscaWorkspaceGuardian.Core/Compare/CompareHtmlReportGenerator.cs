// <copyright file="CompareHtmlReportGenerator.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Compare;

using System.Text;

/// <summary>

/// TODO: Describe CompareHtmlReportGenerator.

/// </summary>

public class CompareHtmlReportGenerator
{
    public async Task GenerateAsync(
        CompareResult result,
        string outputFile)
    {
        var html = new StringBuilder();

        html.AppendLine("<html>");
        html.AppendLine("<head>");
        html.AppendLine("<title>Snapshot Compare Report</title>");
        html.AppendLine("<style>");
        html.AppendLine("body{font-family:Segoe UI;margin:40px;background:#f4f4f4;}");
        html.AppendLine(".card{background:white;padding:20px;margin-bottom:20px;border-radius:8px;}");
        html.AppendLine("table{width:100%;border-collapse:collapse;}");
        html.AppendLine("th,td{border:1px solid #ddd;padding:8px;}");
        html.AppendLine("th{background:#1976d2;color:white;}");
        html.AppendLine("</style>");
        html.AppendLine("</head>");
        html.AppendLine("<body>");

        html.AppendLine("<h1>Snapshot Compare Report</h1>");

        html.AppendLine("<div class='card'>");
        html.AppendLine($"<h3>Added Objects : {result.AddedObjects.Count}</h3>");
        html.AppendLine($"<h3>Removed Objects : {result.RemovedObjects.Count}</h3>");
        html.AppendLine($"<h3>Property Changes : {result.PropertyChanges.Count}</h3>");
        html.AppendLine("</div>");

        html.AppendLine("<div class='card'>");
        html.AppendLine("<h2>Added Objects</h2>");

        foreach (var item in result.AddedObjects)
        {
            html.AppendLine($"<div>{item}</div>");
        }

        html.AppendLine("</div>");

        html.AppendLine("<div class='card'>");
        html.AppendLine("<h2>Removed Objects</h2>");

        foreach (var item in result.RemovedObjects)
        {
            html.AppendLine($"<div>{item}</div>");
        }

        html.AppendLine("</div>");

        html.AppendLine("<div class='card'>");
        html.AppendLine("<h2>Property Changes</h2>");

        html.AppendLine("<table>");
        html.AppendLine("<tr>");
        html.AppendLine("<th>NodePath</th>");
        html.AppendLine("<th>Property</th>");
        html.AppendLine("<th>Old</th>");
        html.AppendLine("<th>New</th>");
        html.AppendLine("</tr>");

        foreach (var p in result.PropertyChanges)
        {
            html.AppendLine("<tr>");
            html.AppendLine($"<td>{p.NodePath}</td>");
            html.AppendLine($"<td>{p.Property}</td>");
            html.AppendLine($"<td>{p.OldValue}</td>");
            html.AppendLine($"<td>{p.NewValue}</td>");
            html.AppendLine("</tr>");
        }

        html.AppendLine("</table>");

        html.AppendLine("</div>");

        html.AppendLine("</body>");
        html.AppendLine("</html>");

        Directory.CreateDirectory(
            Path.GetDirectoryName(outputFile)!);

        await File.WriteAllTextAsync(
            outputFile,
            html.ToString());
    }
}

