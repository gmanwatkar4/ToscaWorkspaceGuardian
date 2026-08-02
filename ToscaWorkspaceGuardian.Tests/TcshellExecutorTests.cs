// <copyright file="TcshellExecutorTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Tests
{
    using System.Threading.Tasks;
    using ToscaWorkspaceGuardian.Common.Interfaces;
    using ToscaWorkspaceGuardian.Common.Models;
    using ToscaWorkspaceGuardian.Common.Utilities;
    using ToscaWorkspaceGuardian.Core.Models;
    using ToscaWorkspaceGuardian.Core.TCShell;
    using Xunit;

    public class TcshellExecutorTests
    {
        private class FakeInstallationService : IToscaInstallationService
        {
            private readonly ToscaInstallation installation;

            public FakeInstallationService(ToscaInstallation installation)
            {
                this.installation = installation;
            }

            public ToscaInstallation GetInstallation() => this.installation;
        }

        private class DummyProcessRunner : IProcessRunner
        {
            public Task<Common.Models.ProcessResult> ExecuteAsync(string fileName, string arguments, System.Threading.CancellationToken cancellationToken = default)
            {
                return Task.FromResult(new Common.Models.ProcessResult
                {
                    ExitCode = 0,
                    StandardOutput = string.Empty,
                    StandardError = string.Empty,
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
