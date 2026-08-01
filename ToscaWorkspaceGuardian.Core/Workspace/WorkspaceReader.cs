using ToscaWorkspaceGuardian.Core.Interfaces;
using ToscaWorkspaceGuardian.Core.Models;
using ToscaWorkspaceGuardian.Core.TCShell;


namespace ToscaWorkspaceGuardian.Core.Workspace;

public class WorkspaceReader : IWorkspaceReader
{
    private readonly IScriptService _scriptService;
    private readonly ITCShellService _tcShellService;
    private readonly OutputDocumentExporter _exporter;
    private readonly IParsedWorkspaceMapper _mapper;

    public WorkspaceReader(
        IScriptService scriptService,
        ITCShellService tcShellService,
        OutputDocumentExporter exporter,
        IParsedWorkspaceMapper mapper)
    {
        _scriptService = scriptService;
        _tcShellService = tcShellService;
        _exporter = exporter;
        _mapper = mapper;
    }

    public async Task<WorkspaceSummary> ReadAsync(
        
        WorkspaceRequest request,
        CancellationToken cancellationToken = default)
    {
        var script = await _scriptService.GenerateScriptAsync(
            new ScriptRequest
            {
                ScriptType = ScriptType.WorkspaceAnalysis,
                WorkspaceRequest = request
            },
            cancellationToken);
       

        var response = await _tcShellService.ExecuteScriptAsync(
            script,
            request,
            cancellationToken);

        var parser = new OutputParser();

        var document = parser.Parse(response.Output);

        await _exporter.ExportAsync(
           document,
            Path.Combine(
            Path.GetTempPath(),
            "ToscaWorkspaceGuardian"));

        var parsedWorkspace =
        _mapper.Map(document);

        return new WorkspaceSummary
        {
            WorkspaceInfo = new WorkspaceInfo
            {
                WorkspacePath = request.WorkspacePath,
                IsManagedRepository = request.IsManagedRepository
            },

            Metadata = new WorkspaceMetadata
            {
                AnalysisTime = DateTime.Now
            },

            Statistics = new WorkspaceStatistics()
        };
    }
}