namespace ToscaWorkspaceGuardian.Core.TQL;

using ToscaWorkspaceGuardian.Core.Models;
using ToscaWorkspaceGuardian.Core.Interfaces;
using ToscaWorkspaceGuardian.Core.TCShell;

/// <summary>
/// Executes a TQL expression through a generated, read-only TCShell script.
/// </summary>
public sealed class TqlQueryRunner : ITqlQueryRunner
{
    private readonly ITCShellService tcShellService;
    private readonly OutputParser outputParser;

    public TqlQueryRunner(ITCShellService tcShellService, OutputParser outputParser)
    {
        this.tcShellService = tcShellService;
        this.outputParser = outputParser;
    }

    public async Task<TqlQueryResult> RunAsync(
        string query,
        WorkspaceRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateQuery(query);

        var runFolder = Path.Combine(Path.GetTempPath(), "ToscaWorkspaceGuardian", "Tql", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(runFolder);
        var eachScript = Path.Combine(runFolder, "PrintCurrentObject.tcs");
        var queryScript = Path.Combine(runFolder, "RunQuery.tcs");
        await File.WriteAllTextAsync(eachScript, "Print\r\n");
        await File.WriteAllTextAsync(queryScript, BuildQueryScript(query, eachScript));

        var execution = await this.tcShellService.ExecuteScriptAsync(queryScript, request, cancellationToken);
        if (!execution.Success)
        {
            return new TqlQueryResult
            {
                Query = query,
                Message = string.IsNullOrWhiteSpace(execution.Error)
                    ? "TCShell did not complete the TQL query."
                    : execution.Error,
            };
        }

        var document = this.outputParser.Parse(execution.Output);
        return new TqlQueryResult
        {
            Success = true,
            Query = query,
            Document = document,
            Message = $"TQL query completed: {document.Objects.Count:N0} object(s) returned.",
        };
    }

    public static string BuildQueryScript(string query, string eachScriptPath)
    {
        var escapedQuery = query.Replace("\"", "\\\"", StringComparison.Ordinal);
        var escapedPath = eachScriptPath.Replace("\\", "\\\\", StringComparison.Ordinal).Replace("\"", "\\\"", StringComparison.Ordinal);
        return $"JumpToProject\r\nFor \"{escapedQuery}\" CallOnEach \"{escapedPath}\"\r\nExit\r\n";
    }

    private static void ValidateQuery(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            throw new ArgumentException("Enter a TQL query.", nameof(query));
        }

        if (query.Contains('\r') || query.Contains('\n'))
        {
            throw new ArgumentException("TQL queries must be a single line.", nameof(query));
        }
    }
}
