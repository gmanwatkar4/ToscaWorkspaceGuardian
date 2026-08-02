using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Windows;
using ToscaWorkspaceGuardian.Common.Interfaces;
using ToscaWorkspaceGuardian.Common.Services;
using ToscaWorkspaceGuardian.Common.Utilities;
using ToscaWorkspaceGuardian.Core.Business;
using ToscaWorkspaceGuardian.Core.Export;
using ToscaWorkspaceGuardian.Core.Health;
using ToscaWorkspaceGuardian.Core.Health.Rules;
using ToscaWorkspaceGuardian.Core.Interfaces;
using ToscaWorkspaceGuardian.Core.Rules;
using ToscaWorkspaceGuardian.Core.Script;
using ToscaWorkspaceGuardian.Core.TCShell;
using ToscaWorkspaceGuardian.Core.Traversal;
using ToscaWorkspaceGuardian.Core.Workspace;
using ToscaWorkspaceGuardian.UI.ViewModels;
using ToscaWorkspaceGuardian.UI.Views;

namespace ToscaWorkspaceGuardian.UI
{
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
            services.AddSingleton<IWorkspaceHealthAnalyzer,WorkspaceHealthAnalyzer>();
            services.AddSingleton<IWorkspaceRule, WG001_MissingReferencesRule>();
            services.AddSingleton<IWorkspaceRule, WG002_NoUsersRule>();
            services.AddSingleton<IWorkspaceRule, WG003_NoGroupsRule>();
            services.AddSingleton<IWorkspaceRule, WG004_NoRootFoldersRule>();
            services.AddSingleton<RepositoryScanScriptBuilder>();
            services.AddSingleton<PrintObjectScriptBuilder>();
            services.AddSingleton<IRepositoryTreeWalker, RepositoryTreeWalker>();
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

        }
    }
}