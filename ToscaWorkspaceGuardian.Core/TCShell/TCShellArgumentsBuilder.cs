namespace ToscaWorkspaceGuardian.Core.TCShell;

using ToscaWorkspaceGuardian.Core.Models;

/// <summary>
/// Builds TCShell command line arguments without exposing credentials in logs.
/// </summary>
public static class TCShellArgumentsBuilder
{
    public static string Build(WorkspaceRequest request, string scriptFile)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(request.WorkspacePath);
        ArgumentException.ThrowIfNullOrWhiteSpace(scriptFile);

        var arguments = $"-workspace {Quote(request.WorkspacePath)} ";
        if (request.IsManagedRepository)
        {
            if (string.IsNullOrWhiteSpace(request.ClientId) || string.IsNullOrWhiteSpace(request.ClientSecret))
            {
                throw new InvalidOperationException("Client ID and client secret are required for a Tricentis Server Repository workspace.");
            }

            arguments += $"-auth {Quote($"{request.ClientId}:{request.ClientSecret}")} ";
        }
        else if (!string.IsNullOrWhiteSpace(request.Username) || !string.IsNullOrWhiteSpace(request.Password))
        {
            arguments += $"-login {Quote(request.Username)} {Quote(request.Password)} ";
        }

        return arguments + Quote(scriptFile);
    }

    private static string Quote(string value) =>
        $"\"{value.Replace("\"", "\\\"", StringComparison.Ordinal)}\"";
}
