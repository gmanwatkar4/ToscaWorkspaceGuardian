namespace ToscaWorkspaceGuardian.Tests;

using ToscaWorkspaceGuardian.Core.Models;
using ToscaWorkspaceGuardian.Core.TCShell;
using ToscaWorkspaceGuardian.Core.TQL;

public sealed class TCShellArgumentsBuilderTests
{
    [Fact]
    public void Build_UsesLoginForMssqlWorkspace()
    {
        var arguments = TCShellArgumentsBuilder.Build(new WorkspaceRequest
        {
            WorkspacePath = @"C:\Workspace\Demo.tws",
            Username = "Admin",
            Password = "password",
        }, @"C:\Temp\Query.tcs");

        Assert.Contains("-login \"Admin\" \"password\"", arguments, StringComparison.Ordinal);
        Assert.DoesNotContain("-auth", arguments, StringComparison.Ordinal);
    }

    [Fact]
    public void Build_UsesAuthForTsrWorkspace()
    {
        var arguments = TCShellArgumentsBuilder.Build(new WorkspaceRequest
        {
            WorkspacePath = @"C:\Workspace\Tsr.tws",
            IsManagedRepository = true,
            ClientId = "client-id",
            ClientSecret = "client-secret",
        }, @"C:\Temp\Query.tcs");

        Assert.Contains("-auth \"client-id:client-secret\"", arguments, StringComparison.Ordinal);
        Assert.DoesNotContain("-login", arguments, StringComparison.Ordinal);
    }

    [Fact]
    public void BuildQueryScript_ContainsOnlyReadOnlyCommands()
    {
        var script = TqlQueryRunner.BuildQueryScript(
            "=>SUBPARTS:TestCase[Name==\"Smoke\"]",
            @"C:\Temp\PrintCurrentObject.tcs");

        Assert.Contains("JumpToProject", script, StringComparison.Ordinal);
        Assert.Contains("CallOnEach", script, StringComparison.Ordinal);
        Assert.Contains("Exit", script, StringComparison.Ordinal);
        Assert.DoesNotContain("Set ", script, StringComparison.Ordinal);
        Assert.DoesNotContain("Task ", script, StringComparison.Ordinal);
        Assert.DoesNotContain("CheckIn", script, StringComparison.Ordinal);
    }
}
