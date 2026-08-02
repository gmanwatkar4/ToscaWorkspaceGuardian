using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace ToscaWorkspaceGuardian.Core.Diagnostics;

public static class TelemetrySources
{
    public static readonly ActivitySource Activity = new("ToscaWorkspaceGuardian");
    public static readonly Meter Meter = new("ToscaWorkspaceGuardian.Metrics");

    // Example instruments
    public static readonly Counter<long> TCShellCalls = Meter.CreateCounter<long>("tc_shell_calls");
}
