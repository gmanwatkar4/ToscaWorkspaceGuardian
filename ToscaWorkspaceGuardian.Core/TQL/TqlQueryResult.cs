namespace ToscaWorkspaceGuardian.Core.TQL;

using ToscaWorkspaceGuardian.Core.Models;

public sealed class TqlQueryResult
{
    public bool Success { get; init; }

    public string Message { get; init; } = string.Empty;

    public string Query { get; init; } = string.Empty;

    public OutputDocument Document { get; init; } = new();
}
