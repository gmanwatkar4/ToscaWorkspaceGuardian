// <copyright file="App.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.UI
{
    using System.Configuration;
    using System.Net.Http;
    using System.Windows;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using ToscaWorkspaceGuardian.Common.Interfaces;
    using ToscaWorkspaceGuardian.Common.Services;
    using ToscaWorkspaceGuardian.Common.Utilities;
    using ToscaWorkspaceGuardian.Core.AI;
    using ToscaWorkspaceGuardian.Core.Business;
    using ToscaWorkspaceGuardian.Core.Compare;
    using ToscaWorkspaceGuardian.Core.Configuration;
    using ToscaWorkspaceGuardian.Core.Export;
    using ToscaWorkspaceGuardian.Core.Health;
    using ToscaWorkspaceGuardian.Core.Health.Rules;
    using ToscaWorkspaceGuardian.Core.Interfaces;
    using ToscaWorkspaceGuardian.Core.Reporting;
    using ToscaWorkspaceGuardian.Core.Rules;
    using ToscaWorkspaceGuardian.Core.Script;
    using ToscaWorkspaceGuardian.Core.Services;
    using ToscaWorkspaceGuardian.Core.TCShell;
    using ToscaWorkspaceGuardian.Core.Traversal;
    using ToscaWorkspaceGuardian.Core.Upgrade;
    using ToscaWorkspaceGuardian.Core.Workspace;
    using ToscaWorkspaceGuardian.Core.Diagnostics;
    using OpenTelemetry.Trace;
    using OpenTelemetry.Metrics;
    using OpenTelemetry.Resources;
    using ToscaWorkspaceGuardian.UI.ViewModels;
    using ToscaWorkspaceGuardian.UI.Views;

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
        // Configure OpenTelemetry tracing + metrics (console exporter for local dev)
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

        var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

        var openRouterSettings = new OpenRouterSettings
        {
            ApiKey = configuration["OpenRouter:ApiKey"] ?? string.Empty,
            Model = configuration["OpenRouter:Model"] ?? "google/gemini-2.5-flash",
        };

        services.AddSingleton(openRouterSettings);

        services.AddSingleton<IConfiguration>(configuration);

        services.Configure<GeminiSettings>(
            configuration.GetSection("Gemini"));

        ConfigureServices(services);
    })

    .Build();

            await Host.StartAsync();

            var mainWindow = Host.Services.GetRequiredService<MainWindow>();

            mainWindow.Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            if (Host != null)
            {
                await Host.StopAsync();
                Host.Dispose();
            }

            base.OnExit(e);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // ViewModels
            services.AddSingleton<MainViewModel>();

            // Views
            services.AddSingleton<MainWindow>();

            services.AddSingleton<IWorkspaceAnalyzer, WorkspaceAnalyzer>();
            services.AddSingleton<IWorkspaceReader, WorkspaceReader>();
            services.AddSingleton<IProcessRunner, ProcessRunner>();
            services.AddSingleton<IToscaInstallationService, ToscaInstallationService>();
            services.AddSingleton<IWorkspaceDetector, WorkspaceDetector>();
            services.AddSingleton<IScriptService, ScriptService>();
            services.AddSingleton<TCShellExecutor>();
            services.AddSingleton<ITCShellService, TCShellService>();
            services.AddSingleton<ScriptTemplateRepository>();
            services.AddSingleton<ScriptComposer>();
            services.AddSingleton<OutputParser>();
            services.AddSingleton<OutputWriter>();
            services.AddSingleton<OutputDocumentExporter>();
            services.AddSingleton<IParsedWorkspaceMapper, ParsedWorkspaceMapper>();
            services.AddSingleton<IWorkspaceHealthAnalyzer, WorkspaceHealthAnalyzer>();
            services.AddSingleton<IWorkspaceRule, WG001_MissingReferencesRule>();
            services.AddSingleton<IWorkspaceRule, WG002_NoUsersRule>();
            services.AddSingleton<IWorkspaceRule, WG003_NoGroupsRule>();
            services.AddSingleton<IWorkspaceRule, WG004_NoRootFoldersRule>();
            services.AddSingleton<RepositoryScanScriptBuilder>();
            services.AddSingleton<PrintObjectScriptBuilder>();
            services.AddSingleton<IRepositoryTreeWalker, RepositoryTreeWalker>();
            services.AddSingleton<TelemetryCollector>();
            services.AddSingleton<ToscaWorkspaceGuardian.Core.Caching.NodeCacheService>();
            services.AddSingleton<NodePrintScriptBuilder>();
            services.AddSingleton<BatchScriptBuilder>();
            services.AddSingleton<ISnapshotBuilder, SnapshotBuilder>();
            services.AddSingleton<IWorkspaceSnapshotWriter, WorkspaceSnapshotWriter>();
            services.AddSingleton<IWorkspaceCrawler, WorkspaceCrawler>();
            services.AddSingleton<INodePathResolver, NodePathResolver>();
            services.AddSingleton<IWorkspaceSnapshotExporter, WorkspaceSnapshotExporter>();
            services.AddSingleton<IHealthRule, WG001MissingReferencesRule>();
            services.AddSingleton<HealthAnalyzer>();
            services.AddSingleton<IHealthReportExporter, HealthReportExporter>();
            services.AddSingleton<IHealthRule, WG002EmptyFolderRule>();
            services.AddSingleton<IHealthRule, WG003MissingDescriptionRule>();
            services.AddSingleton<IHealthRule, WG004DuplicateNamesRule>();
            services.AddSingleton<RepositoryStatisticsBuilder>();
            services.AddSingleton<WorkspaceHtmlReportGenerator>();
            services.AddSingleton<WorkspaceSnapshotComparer>();
            services.AddSingleton<ICompareReportExporter, JsonCompareReportExporter>();
            services.AddSingleton<CompareHtmlReportGenerator>();
            services.AddSingleton<IWorkspaceCompareService, WorkspaceCompareService>();
            services.AddSingleton<UpgradeReadinessAnalyzer>();
            services.AddSingleton<VersionCompatibilityEngine>();
            services.AddSingleton<ConfigurationLoader>();
            services.AddTransient<GeminiProvider>();
            services.AddSingleton(new HttpClient());
            services.AddSingleton<IAIProvider>(provider =>
            {
                var settings =
                    provider.GetRequiredService<OpenRouterSettings>();

                return new OpenRouterProvider(settings);
            });
        }
    }
}
