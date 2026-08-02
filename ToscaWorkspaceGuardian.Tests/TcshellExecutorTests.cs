using System.Threading.Tasks;
using ToscaWorkspaceGuardian.Common.Interfaces;
using ToscaWorkspaceGuardian.Common.Models;
using ToscaWorkspaceGuardian.Common.Utilities;
using ToscaWorkspaceGuardian.Core.TCShell;
using ToscaWorkspaceGuardian.Core.Models;
using Xunit;

namespace ToscaWorkspaceGuardian.Tests
{
    public class TcshellExecutorTests
    {
        private class FakeInstallationService : IToscaInstallationService
        {
            private readonly ToscaInstallation _installation;

            public FakeInstallationService(ToscaInstallation installation)
            {
                _installation = installation;
            }

            public ToscaInstallation GetInstallation() => _installation;
        }

        private class DummyProcessRunner : IProcessRunner
        {
            public Task<Common.Models.ProcessResult> ExecuteAsync(string fileName, string arguments, System.Threading.CancellationToken cancellationToken = default)
            {
                return Task.FromResult(new Common.Models.ProcessResult
                {
                    ExitCode = 0,
                    StandardOutput = string.Empty,
                    StandardError = string.Empty
                });
            }
        }

        [Fact]
        public async Task ExecuteAsync_ReturnsFailure_WhenTcshellNotInstalled()
        {
            var installation = new ToscaInstallation { IsInstalled = false };
            var installationService = new FakeInstallationService(installation);
            var processRunner = new DummyProcessRunner();
            var outputWriter = new OutputWriter();

            var executor = new TCShellExecutor(installationService, processRunner, outputWriter);

            var result = await executor.ExecuteAsync("script.tcs", new WorkspaceRequest(), cancellationToken: System.Threading.CancellationToken.None);

            Assert.False(result.Success);
            Assert.Equal(-1, result.ExitCode);
            Assert.Contains("not found", result.StandardError ?? string.Empty);
        }
    }
}
