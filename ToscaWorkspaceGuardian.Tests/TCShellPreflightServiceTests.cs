namespace ToscaWorkspaceGuardian.Tests;

using ToscaWorkspaceGuardian.Common.Interfaces;
using ToscaWorkspaceGuardian.Common.Models;
using ToscaWorkspaceGuardian.Core.TCShell;

public sealed class TCShellPreflightServiceTests
{
    [Fact]
    public async Task CheckAsync_ReturnsReadyWhenHelpReportsLicense()
    {
        var service = new TCShellPreflightService(
            new StubInstallationService(),
            new StubProcessRunner(new ProcessResult { ExitCode = 0, StandardOutput = "Checking License ... License found!" }));

        var result = await service.CheckAsync();

        Assert.True(result.IsReady);
        Assert.True(result.LicenseDetected);
        Assert.Contains("license", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    private sealed class StubInstallationService : IToscaInstallationService
    {
        public ToscaInstallation GetInstallation() => new()
        {
            IsInstalled = true,
            TCShellPath = @"C:\Tosca\TCShell.exe",
        };
    }

    private sealed class StubProcessRunner : IProcessRunner
    {
        private readonly ProcessResult result;

        public StubProcessRunner(ProcessResult result)
        {
            this.result = result;
        }

        public Task<ProcessResult> ExecuteAsync(string fileName, string arguments, CancellationToken cancellationToken = default)
        {
            Assert.Equal(@"C:\Tosca\TCShell.exe", fileName);
            Assert.Equal("-help", arguments);
            return Task.FromResult(this.result);
        }
    }
}
