using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Windows;
using ToscaWorkspaceGuardian.Common.Interfaces;
using ToscaWorkspaceGuardian.Common.Services;
using ToscaWorkspaceGuardian.Common.Utilities;
using ToscaWorkspaceGuardian.Core.Interfaces;
using ToscaWorkspaceGuardian.Core.Script;
using ToscaWorkspaceGuardian.Core.TCShell;
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

            // Services
            // Register later
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





        }
    }
}