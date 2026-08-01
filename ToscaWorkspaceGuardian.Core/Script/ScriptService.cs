using ToscaWorkspaceGuardian.Core.Interfaces;
using ToscaWorkspaceGuardian.Core.Models;

namespace ToscaWorkspaceGuardian.Core.Script;

public class ScriptService : IScriptService
{
    private readonly ScriptComposer _composer;

    public ScriptService(
        ScriptComposer composer)
    {
        _composer = composer;
    }

    public async Task<string> GenerateScriptAsync(
        ScriptRequest request,
        CancellationToken cancellationToken = default)
    {
        string script =
            _composer.Compose(request);

        string directory =
            Path.Combine(
                Path.GetTempPath(),
                "ToscaWorkspaceGuardian");

        Directory.CreateDirectory(directory);

        string file =
            Path.Combine(
                directory,
                "WorkspaceAnalysis.tcs");

        await File.WriteAllTextAsync(
            file,
            script,
            cancellationToken);

        return file;
    }
}