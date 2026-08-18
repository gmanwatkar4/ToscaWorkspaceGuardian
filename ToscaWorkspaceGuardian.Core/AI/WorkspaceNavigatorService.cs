namespace ToscaWorkspaceGuardian.Core.AI;

using System.Text;
using System.Text.RegularExpressions;
using ToscaWorkspaceGuardian.Core.Models;
using ToscaWorkspaceGuardian.Core.TQL;

/// <summary>
/// Converts natural-language workspace questions into validated, read-only TQL searches.
/// </summary>
public sealed class WorkspaceNavigatorService : IWorkspaceNavigatorService
{
    private static readonly Regex UnsafeToken = new(@"\b(set|task|call|exit|save|checkin|update|delete|mark|drop)\b", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    private readonly IAIProvider aiProvider;
    private readonly ITqlQueryRunner queryRunner;

    public WorkspaceNavigatorService(IAIProvider aiProvider, ITqlQueryRunner queryRunner)
    {
        this.aiProvider = aiProvider;
        this.queryRunner = queryRunner;
    }

    public async Task<WorkspaceNavigationResponse> AskAsync(
        string question,
        WorkspaceRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(question);

        var queryResponse = await this.aiProvider.GenerateAsync(
            new AIRequest { Prompt = BuildTqlPrompt(question) },
            cancellationToken);
        var query = ExtractTql(queryResponse.Content);
        ValidateReadOnlyQuery(query);

        var queryResult = await this.queryRunner.RunAsync(query, request, cancellationToken);
        if (!queryResult.Success)
        {
            return new WorkspaceNavigationResponse
            {
                GeneratedTql = query,
                Answer = queryResult.Message,
            };
        }

        var answerResponse = await this.aiProvider.GenerateAsync(
            new AIRequest { Prompt = BuildAnswerPrompt(question, query, queryResult.Document) },
            cancellationToken);
        return new WorkspaceNavigationResponse
        {
            Success = true,
            GeneratedTql = query,
            Results = queryResult.Document,
            Answer = string.IsNullOrWhiteSpace(answerResponse.Content)
                ? $"The query returned {queryResult.Document.Objects.Count} object(s)."
                : answerResponse.Content.Trim(),
        };
    }

    public static void ValidateReadOnlyQuery(string query)
    {
        if (string.IsNullOrWhiteSpace(query) || (!query.StartsWith("=>", StringComparison.Ordinal) && !query.StartsWith("->", StringComparison.Ordinal)))
        {
            throw new InvalidOperationException("Copilot did not generate a valid TQL traversal query.");
        }

        if (query.Contains('\r') || query.Contains('\n') || UnsafeToken.IsMatch(query))
        {
            throw new InvalidOperationException("Copilot generated an unsafe query. Only read-only TQL traversal queries are allowed.");
        }
    }

    private static string ExtractTql(string content)
    {
        var line = content.Replace("```tql", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("```", string.Empty, StringComparison.Ordinal)
            .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(item => item.Trim())
            .FirstOrDefault(item => item.StartsWith("=>", StringComparison.Ordinal) || item.StartsWith("->", StringComparison.Ordinal));
        return line ?? string.Empty;
    }

    private static string BuildTqlPrompt(string question) =>
        $"""
        You translate a Tosca workspace question into exactly one read-only TQL query.
        Return only the query on one line; no prose, markdown, or explanation.
        The query must start with => or -> and use only TQL traversal, type filters, attribute filters, logical operators, and functions.
        Never return TCShell commands or tokens such as Set, Task, Call, Exit, Save, CheckIn, Update, Delete, Mark, or Drop.
        Question: {question}
        """;

    private static string BuildAnswerPrompt(string question, string query, OutputDocument document)
    {
        var evidence = new StringBuilder();
        foreach (var item in document.Objects.Take(30))
        {
            evidence.AppendLine($"- {item.ObjectType}: {item.Name}");
        }

        return $"""
            You are a Tosca support copilot. Answer only from the TQL evidence supplied below.
            Do not invent object names, relationships, counts, upgrade behavior, or fixes. If the evidence is insufficient, state which query is needed next.

            User question: {question}
            Generated TQL: {query}
            Returned object count: {document.Objects.Count}
            Evidence:
            {evidence}

            Give a concise support answer with: Result, Evidence, Next step.
            """;
    }
}
