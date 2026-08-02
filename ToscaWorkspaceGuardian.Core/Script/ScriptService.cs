using ToscaWorkspaceGuardian.Core.Interfaces;
using ToscaWorkspaceGuardian.Core.Models;

namespace ToscaWorkspaceGuardian.Core.Script;

public class ScriptService : IScriptService
{
    private readonly ScriptComposer _composer;
    private readonly RepositoryScanScriptBuilder _repositoryBuilder;
    private readonly PrintObjectScriptBuilder _printBuilder;

    public ScriptService(
        ScriptComposer composer,
        RepositoryScanScriptBuilder repositoryBuilder,
        PrintObjectScriptBuilder printBuilder)
    {
        _composer = composer;
        _repositoryBuilder = repositoryBuilder;
        _printBuilder = printBuilder;
    }

    public async Task<string> GenerateScriptAsync(
        ScriptRequest request,
        CancellationToken cancellationToken = default)
    {
        string directory =
            Path.Combine(
                Path.GetTempPath(),
                "ToscaWorkspaceGuardian");

        Directory.CreateDirectory(directory);

        if (request.ScriptType == ScriptType.RepositoryScan)
        {
            string repositoryScript =
                _repositoryBuilder.Build(request.Queries);

            string repositoryFile =
                Path.Combine(directory, "RepositoryScan.tcs");

            await File.WriteAllTextAsync(
                repositoryFile,
                repositoryScript,
                cancellationToken);

            string printFile =
                Path.Combine(directory, "PrintObject.tcs");

            await File.WriteAllTextAsync(
                printFile,
                _printBuilder.Build(),
                cancellationToken);

            return repositoryFile;
        }

        string script =
            _composer.Compose(request);

        string workspaceFile =
            Path.Combine(directory, "WorkspaceAnalysis.tcs");

        await File.WriteAllTextAsync(
            workspaceFile,
            script,
            cancellationToken);

        return workspaceFile;
    }
}