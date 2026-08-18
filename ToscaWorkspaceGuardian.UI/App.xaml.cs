namespace ToscaWorkspaceGuardian.UI;

using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using ToscaWorkspaceGuardian.Common.Interfaces;
using ToscaWorkspaceGuardian.Common.Services;
using ToscaWorkspaceGuardian.Common.Utilities;
using ToscaWorkspaceGuardian.Core.AI;
using ToscaWorkspaceGuardian.Core.Business;
using ToscaWorkspaceGuardian.Core.Compare;
using ToscaWorkspaceGuardian.Core.Diagnostics;
using ToscaWorkspaceGuardian.Core.Export;
using ToscaWorkspaceGuardian.Core.Health;
using ToscaWorkspaceGuardian.Core.Health.Rules;
using ToscaWorkspaceGuardian.Core.Interfaces;
using ToscaWorkspaceGuardian.Core.Reporting;
using ToscaWorkspaceGuardian.Core.Repository;
using ToscaWorkspaceGuardian.Core.Script;
using ToscaWorkspaceGuardian.Core.Services;
using ToscaWorkspaceGuardian.Core.Subset;
using ToscaWorkspaceGuardian.Core.TCShell;
using ToscaWorkspaceGuardian.Core.TQL;
using ToscaWorkspaceGuardian.Core.Traversal;
using ToscaWorkspaceGuardian.Core.Upgrade;
using ToscaWorkspaceGuardian.Core.Workspace;
using ToscaWorkspaceGuardian.UI.ViewModels;
using ToscaWorkspaceGuardian.UI.Views;
using ToscaWorkspaceGuardian.UI.Services;

public partial class App : Application
{
    public static IHost? Host { get; private set; }

    protected override async void OnStartup(StartupEventArgs e)
    {
        Host = Microsoft.Extensions.Hosting.Host
            .CreateDefaultBuilder()
            .ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddDebug();
            })
            .ConfigureServices((context, services) =>
            {
                services.AddOpenTelemetryTracing(builder =>
                {
                    builder.AddSource("ToscaWorkspaceGuardian");
                    builder.AddConsoleExporter();
                });

                services.AddOpenTelemetryMetrics(builder =>
                {
                    builder.AddMeter("ToscaWorkspaceGuardian.Metrics");
                    builder.AddConsoleExporter();
                });

                ConfigureServices(services, context.Configuration["OpenRouter:ApiKey"], context.Configuration["OpenRouter:Model"]);
            })
            .Build();

        await Host.StartAsync();

        var mainWindow = Host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (Host is not null)
        {
            await Host.StopAsync();
            Host.Dispose();
        }

        base.OnExit(e);
    }

    private static void ConfigureServices(IServiceCollection services, string? openRouterApiKey, string? openRouterModel)
    {
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<MainWindow>();
        services.AddSingleton<IUserPreferencesService, UserPreferencesService>();

        services.AddSingleton<IToscaInstallationService, ToscaInstallationService>();
        services.AddSingleton<IProcessRunner, ProcessRunner>();
        services.AddSingleton<IWorkspaceDetector, WorkspaceDetector>();

        services.AddSingleton<TelemetryCollector>();
        services.AddSingleton<OutputWriter>();
        services.AddSingleton<TCShellExecutor>();
        services.AddSingleton<ITCShellService, TCShellService>();
        services.AddSingleton<ITCShellPreflightService, TCShellPreflightService>();
        services.AddSingleton<IWorkspaceReadActivity, WorkspaceReadActivity>();
        services.AddSingleton<IWorkspaceInventoryService, WorkspaceInventoryService>();
        services.AddSingleton<ITqlQueryRunner, TqlQueryRunner>();
        services.AddSingleton<IUpgradeRiskScanner, TqlUpgradeRiskScanner>();
        services.AddSingleton<IInventoryUpgradeRuleCatalog, InventoryUpgradeRuleCatalog>();
        services.AddSingleton<IInventoryUpgradeRiskScanner, InventoryUpgradeRiskScanner>();
        services.AddSingleton<BatchScriptBuilder>();
        services.AddSingleton<OutputParser>();
        services.AddSingleton<ISnapshotBuilder, SnapshotBuilder>();
        services.AddSingleton<IWorkspaceCrawler, WorkspaceCrawler>();

        services.AddSingleton<IWorkspaceSnapshotExporter, WorkspaceSnapshotExporter>();
        services.AddSingleton<IHealthRule, WG001MissingReferencesRule>();
        services.AddSingleton<IHealthRule, WG002EmptyFolderRule>();
        services.AddSingleton<IHealthRule, WG003MissingDescriptionRule>();
        services.AddSingleton<IHealthRule, WG004DuplicateNamesRule>();
        services.AddSingleton<HealthAnalyzer>();
        services.AddSingleton<IHealthReportExporter, HealthReportExporter>();
        services.AddSingleton<RepositoryStatisticsBuilder>();
        services.AddSingleton<WorkspaceHtmlReportGenerator>();
        services.AddSingleton<UpgradeRiskHtmlReportGenerator>();
        services.AddSingleton<UpgradeComparisonHtmlReportGenerator>();
        services.AddSingleton<VersionCompatibilityEngine>();
        services.AddSingleton<UpgradeReadinessAnalyzer>();
        services.AddSingleton<UpgradeRiskRuleCatalog>();

        if (string.IsNullOrWhiteSpace(openRouterApiKey))
        {
            services.AddSingleton<IAIProvider, MockAIProvider>();
        }
        else
        {
            services.AddSingleton(new OpenRouterSettings
            {
                ApiKey = openRouterApiKey,
                Model = string.IsNullOrWhiteSpace(openRouterModel) ? "google/gemini-2.5-flash" : openRouterModel,
            });
            services.AddSingleton<IAIProvider, OpenRouterProvider>();
        }

        services.AddSingleton<IUpgradeCopilotService, UpgradeCopilotService>();
        services.AddSingleton<IInventoryCopilotService, InventoryCopilotService>();
        services.AddSingleton<IWorkspaceNavigatorService, WorkspaceNavigatorService>();
        services.AddSingleton<IWorkspaceAnalyzer, WorkspaceAnalyzer>();
        services.AddSingleton<IToscaSubsetAnalyzer, ToscaSubsetAnalyzer>();
        services.AddSingleton<IToscaRepositoryDatabaseAnalyzer, ToscaRepositoryDatabaseAnalyzer>();
        services.AddSingleton<WorkspaceSnapshotComparer>();
        services.AddSingleton<IWorkspaceCompareService, WorkspaceCompareService>();
        services.AddSingleton<ICompareReportExporter, JsonCompareReportExporter>();
        services.AddSingleton<CompareHtmlReportGenerator>();
    }
}
