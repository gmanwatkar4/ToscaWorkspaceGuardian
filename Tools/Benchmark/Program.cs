<!--
// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
-->
using System.Diagnostics;
using ToscaWorkspaceGuardian.Core.TCShell;

// Simple synthetic benchmark that measures OutputParser performance
Console.WriteLine("Benchmark: Starting");

// create a large synthetic output containing repeated object blocks
var sb = new System.Text.StringBuilder();
for (int i = 0; i < 5000; i++)
{
    sb.AppendLine($"'Object{i}' [TCFolder]");
    sb.AppendLine("Name='Sample'");
    sb.AppendLine("NodePath='/Modules/Module1'");
    sb.AppendLine("Items : { 'Child1','Child2','Child3' }");
    sb.AppendLine();
}

var input = sb.ToString();

var parser = new OutputParser();

var sw = Stopwatch.StartNew();
var doc = parser.Parse(input);
sw.Stop();

Console.WriteLine($"Parsed objects: {doc.Objects.Count}; time_ms={sw.Elapsed.TotalMilliseconds:0.0}");

// write results to artifact file
var outDir = Path.Combine(Path.GetTempPath(), "t wg_benchmark");
Directory.CreateDirectory(outDir);
var resultFile = Path.Combine(outDir, "benchmark_result.txt");
await File.WriteAllTextAsync(resultFile, $"Parsed={doc.Objects.Count},ms={sw.Elapsed.TotalMilliseconds:0.0}\n");

Console.WriteLine($"Benchmark: result written to {resultFile}");

