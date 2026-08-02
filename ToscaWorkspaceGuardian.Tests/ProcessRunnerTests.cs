// <copyright file="ProcessRunnerTests.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Tests
{
    using System.Threading.Tasks;
    using ToscaWorkspaceGuardian.Common.Utilities;
    using Xunit;

    public class ProcessRunnerTests
    {
        [Fact]
        public async Task ExecuteAsync_ReturnsOutput_ForDotnetVersion()
        {
            var runner = new ProcessRunner();

            var result = await runner.ExecuteAsync("dotnet", "--version");

            Assert.NotNull(result);
            Assert.Equal(0, result.ExitCode);
            Assert.False(string.IsNullOrWhiteSpace(result.StandardOutput));
        }
    }
}
