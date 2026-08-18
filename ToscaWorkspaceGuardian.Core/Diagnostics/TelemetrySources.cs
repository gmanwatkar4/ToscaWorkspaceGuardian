// <copyright file="TelemetrySources.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.Core.Diagnostics;

using System.Diagnostics;
using System.Diagnostics.Metrics;

public static class TelemetrySources
{
    public static readonly ActivitySource Activity = new("ToscaWorkspaceGuardian");
    public static readonly Meter Meter = new("ToscaWorkspaceGuardian.Metrics");

    // Example instruments
    public static readonly Counter<long> TCShellCalls = Meter.CreateCounter<long>("tc_shell_calls");
}
