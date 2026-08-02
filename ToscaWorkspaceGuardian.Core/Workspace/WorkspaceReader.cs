// <copyright file="WorkspaceReader.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Workspace;

using ToscaWorkspaceGuardian.Core.Interfaces;
using ToscaWorkspaceGuardian.Core.Models;
using ToscaWorkspaceGuardian.Core.TCShell;

/// <summary>

/// TODO: Describe WorkspaceReader.

/// </summary>

public class WorkspaceReader : IWorkspaceReader
{
    private readonly IScriptService scriptService;
    private readonly ITCShellService tcShellService;
    private readonly OutputDocumentExporter exporter;
    private readonly IParsedWorkspaceMapper mapper;

    public WorkspaceReader(
        IScriptService scriptService,
        ITCShellService tcShellService,
        OutputDocumentExporter exporter,
        IParsedWorkspaceMapper mapper)
    {
        this.scriptService = scriptService;
        this.tcShellService = tcShellService;
        this.exporter = exporter;
        this.mapper = mapper;
    }

    public async Task<WorkspaceSummary> ReadAsync(

        WorkspaceRequest request,
        CancellationToken cancellationToken = default)
    {
        var script = await this.scriptService.GenerateScriptAsync(
     new ScriptRequest
     {
         ScriptType = ScriptType.RepositoryScan,
         WorkspaceRequest = request,

         Queries =
         {
            "=>SUBPARTS:Module",
            "=>SUBPARTS:TestCase",
            "=>SUBPARTS:ExecutionList",
            "=>SUBPARTS:Requirement",
         },
     },
     cancellationToken);

        var response = await this.tcShellService.ExecuteScriptAsync(
            script,
            request,
            cancellationToken);

        var parser = new OutputParser();

        var document = parser.Parse(response.Output);

        await this.exporter.ExportAsync(
           document,
           Path.Combine(
            Path.GetTempPath(),
            "ToscaWorkspaceGuardian"));

        var parsedWorkspace =
        this.mapper.Map(document);

        return new WorkspaceSummary
        {
            WorkspaceInfo = new WorkspaceInfo
            {
                WorkspacePath = request.WorkspacePath,
                IsManagedRepository = request.IsManagedRepository,
            },

            Metadata = new WorkspaceMetadata
            {
                AnalysisTime = DateTime.Now,
            },

            Statistics = new WorkspaceStatistics(),
        };
    }
}

