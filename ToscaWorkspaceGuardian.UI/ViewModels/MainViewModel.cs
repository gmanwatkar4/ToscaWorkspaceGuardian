// <copyright file="MainViewModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace ToscaWorkspaceGuardian.UI.ViewModels;

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows.Data;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Data.Sqlite;
using Microsoft.Win32;
using ToscaWorkspaceGuardian.Core.AI;
using ToscaWorkspaceGuardian.Common.Interfaces;
using ToscaWorkspaceGuardian.Core.Business;
using ToscaWorkspaceGuardian.Core.Compare;
using ToscaWorkspaceGuardian.Core.Interfaces;
using ToscaWorkspaceGuardian.Core.Models;
using ToscaWorkspaceGuardian.Core.Repository;
using ToscaWorkspaceGuardian.Core.Reporting;
using ToscaWorkspaceGuardian.Core.Services;
using ToscaWorkspaceGuardian.Core.Subset;
using ToscaWorkspaceGuardian.Core.TCShell;
using ToscaWorkspaceGuardian.Core.TQL;
using ToscaWorkspaceGuardian.Core.Upgrade;
using ToscaWorkspaceGuardian.Core.Workspace;
using ToscaWorkspaceGuardian.UI.Services;

public partial class MainViewModel : ObservableObject
{
    private readonly IToscaInstallationService installationService;
    private readonly IWorkspaceDetector workspaceDetector;
    private readonly IWorkspaceAnalyzer workspaceAnalyzer;
    private readonly IWorkspaceCompareService workspaceCompareService;
    private readonly ICompareReportExporter compareReportExporter;
    private readonly CompareHtmlReportGenerator compareHtmlReportGenerator;
    private readonly IUserPreferencesService userPreferencesService;
    private readonly IToscaSubsetAnalyzer subsetAnalyzer;
    private readonly IToscaRepositoryDatabaseAnalyzer repositoryDatabaseAnalyzer;
    private readonly ITCShellPreflightService tcShellPreflightService;
    private readonly IWorkspaceInventoryService workspaceInventoryService;
    private readonly IWorkspaceReadActivity workspaceReadActivity;
    private readonly ITqlQueryRunner tqlQueryRunner;
    private readonly IUpgradeCopilotService upgradeCopilotService;
    private readonly IUpgradeRiskScanner upgradeRiskScanner;
    private readonly IInventoryUpgradeRiskScanner inventoryUpgradeRiskScanner;
    private readonly UpgradeRiskHtmlReportGenerator upgradeRiskReportGenerator;
    private readonly UpgradeComparisonHtmlReportGenerator upgradeComparisonReportGenerator;
    private readonly IWorkspaceNavigatorService workspaceNavigatorService;
    private readonly IInventoryCopilotService inventoryCopilotService;
    private AnalysisResult? lastAnalysisResult;
    private CancellationTokenSource? analysisCancellationSource;
    private CancellationTokenSource? workspaceInventoryCancellationSource;
    private readonly ICollectionView healthIssuesView;

    // ====================================================
    // Versions
    // ====================================================
    [ObservableProperty]
    private string sourceVersion = "2025.1";

    [ObservableProperty]
    private string targetVersion = "2026.1";

    // ====================================================
    // Workspace
    // ====================================================
    [ObservableProperty]
    private string workspacePath = string.Empty;

    [ObservableProperty]
    private string outputFolder = string.Empty;

    // ====================================================
    // Repository
    // ====================================================
    [ObservableProperty]
    private string repositoryType = "Not Detected";

    [ObservableProperty]
    private string workspaceAccessStatus = "Select a workspace to determine its authentication requirements.";

    [ObservableProperty]
    private string projectId = string.Empty;

    // ====================================================
    // SQL Authentication
    // ====================================================
    [ObservableProperty]
    private bool showSqlAuthentication;

    [ObservableProperty]
    private string username = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    // ====================================================
    // TSR Authentication
    // ====================================================
    [ObservableProperty]
    private bool showTsrAuthentication;

    [ObservableProperty]
    private string clientId = string.Empty;

    [ObservableProperty]
    private string clientSecret = string.Empty;

    // ====================================================
    // Status
    // ====================================================
    [ObservableProperty]
    private string statusMessage = "Ready";

    [ObservableProperty]
    private bool isAnalyzing;

    [ObservableProperty]
    private int totalObjects;

    [ObservableProperty]
    private int healthScore;

    [ObservableProperty]
    private int healthIssueCount;

    [ObservableProperty]
    private int blockingIssues;

    [ObservableProperty]
    private int warnings;

    [ObservableProperty]
    private string upgradeStatus = "Not analyzed";

    [ObservableProperty]
    private string reportPath = string.Empty;

    [ObservableProperty]
    private int analysisProgress;

    [ObservableProperty]
    private string analysisProgressMessage = "Ready";

    [ObservableProperty]
    private string issueSearchText = string.Empty;

    [ObservableProperty]
    private string selectedSeverity = "All";

    [ObservableProperty]
    private string baselineSnapshotPath = string.Empty;

    [ObservableProperty]
    private string currentSnapshotPath = string.Empty;

    [ObservableProperty]
    private int addedObjectCount;

    [ObservableProperty]
    private int removedObjectCount;

    [ObservableProperty]
    private int changedPropertyCount;

    [ObservableProperty]
    private string compareStatus = "Select two snapshot files to compare.";

    [ObservableProperty]
    private string compareReportPath = string.Empty;

    [ObservableProperty]
    private string preferencesStatus = "Preferences are stored locally on this computer.";

    [ObservableProperty]
    private string subsetPath = string.Empty;

    [ObservableProperty]
    private string subsetStatus = "Select a Tosca subset (.tsu) to analyze.";

    [ObservableProperty]
    private int subsetEntityCount;

    [ObservableProperty]
    private string subsetProjectName = string.Empty;

    [ObservableProperty]
    private string repositoryDatabasePath = string.Empty;

    [ObservableProperty]
    private string repositoryDatabaseStatus = "Select a local Tosca repository database (.db).";

    [ObservableProperty]
    private int repositoryDatabaseObjectCount;

    [ObservableProperty]
    private int repositoryDatabaseTypeCount;

    [ObservableProperty]
    private string tcShellStatus = "TCShell preflight has not been run.";

    [ObservableProperty]
    private bool tcShellReady;

    [ObservableProperty]
    private bool isWorkspaceInventoryAnalyzing;

    [ObservableProperty]
    private string workspaceInventoryStatus = "Select a workspace and verify TCShell before analyzing it.";

    [ObservableProperty]
    private string workspaceInventoryPath = string.Empty;

    [ObservableProperty]
    private int workspaceInventoryObjectCount;

    [ObservableProperty]
    private string tqlQuery = "=>SUBPARTS:TestCase";

    [ObservableProperty]
    private string tqlQueryStatus = "Enter a read-only TQL query after selecting a workspace.";

    [ObservableProperty]
    private string copilotBriefing = "Run an upgrade scan to generate an AI support briefing from verified findings.";

    [ObservableProperty]
    private string copilotQuestion = string.Empty;

    [ObservableProperty]
    private string copilotAnswer = string.Empty;

    [ObservableProperty]
    private bool isCopilotWorking;

    [ObservableProperty]
    private string upgradeRiskScanStatus = "Run TCShell preflight, then scan this workspace for 2026.1 upgrade risks.";

    [ObservableProperty]
    private string upgradeRiskReportPath = string.Empty;

    [ObservableProperty]
    private string inventoryUpgradeScanStatus = "Create an inventory, then evaluate documented upgrade rules without reading the workspace again.";

    [ObservableProperty]
    private bool isInventoryUpgradeScanning;

    [ObservableProperty]
    private bool includeExecutionHealth;

    [ObservableProperty]
    private string navigatorQuestion = string.Empty;

    [ObservableProperty]
    private string navigatorGeneratedTql = string.Empty;

    [ObservableProperty]
    private string navigatorAnswer = "Ask Copilot a workspace question to generate and run a safe live TQL search.";

    [ObservableProperty]
    private string inventoryQuestion = string.Empty;

    [ObservableProperty]
    private string inventoryAiAnswer = "Ask a question about the saved inventory. This does not start TCShell or reread the workspace.";

    [ObservableProperty]
    private bool isInventoryAiWorking;

    public ObservableCollection<HealthIssue> HealthIssues { get; } = new();

    public ObservableCollection<string> UpgradeRecommendations { get; } = new();

    public ObservableCollection<SubsetFinding> SubsetFindings { get; } = new();

    public ObservableCollection<RepositoryTypeCount> RepositoryTypeCounts { get; } = new();

    public ObservableCollection<SubsetFinding> RepositoryDatabaseFindings { get; } = new();

    public ObservableCollection<OutputObject> TqlResults { get; } = new();

    public ObservableCollection<UpgradeRiskFinding> UpgradeRiskFindings { get; } = new();

    public ObservableCollection<OutputObject> NavigatorResults { get; } = new();

    public ObservableCollection<WorkspaceActivityEntry> WorkspaceActivity { get; } = new();

    public ICollectionView HealthIssuesView => this.healthIssuesView;

    public IReadOnlyList<string> SeverityOptions { get; } = new[] { "All", "High", "Medium", "Low", "Info" };

    public MainViewModel(
        IToscaInstallationService installationService,
        IWorkspaceDetector workspaceDetector,
        IWorkspaceAnalyzer workspaceAnalyzer,
        IWorkspaceCompareService workspaceCompareService,
        ICompareReportExporter compareReportExporter,
        CompareHtmlReportGenerator compareHtmlReportGenerator,
        IUserPreferencesService userPreferencesService,
        IToscaSubsetAnalyzer subsetAnalyzer,
        IToscaRepositoryDatabaseAnalyzer repositoryDatabaseAnalyzer,
        ITCShellPreflightService tcShellPreflightService,
        IWorkspaceInventoryService workspaceInventoryService,
        IWorkspaceReadActivity workspaceReadActivity,
        ITqlQueryRunner tqlQueryRunner,
        IUpgradeCopilotService upgradeCopilotService,
        IUpgradeRiskScanner upgradeRiskScanner,
        IInventoryUpgradeRiskScanner inventoryUpgradeRiskScanner,
        UpgradeRiskHtmlReportGenerator upgradeRiskReportGenerator,
        UpgradeComparisonHtmlReportGenerator upgradeComparisonReportGenerator,
        IWorkspaceNavigatorService workspaceNavigatorService,
        IInventoryCopilotService inventoryCopilotService)
    {
        this.installationService = installationService;
        this.workspaceDetector = workspaceDetector;
        this.workspaceAnalyzer = workspaceAnalyzer;
        this.workspaceCompareService = workspaceCompareService;
        this.compareReportExporter = compareReportExporter;
        this.compareHtmlReportGenerator = compareHtmlReportGenerator;
        this.userPreferencesService = userPreferencesService;
        this.subsetAnalyzer = subsetAnalyzer;
        this.repositoryDatabaseAnalyzer = repositoryDatabaseAnalyzer;
        this.tcShellPreflightService = tcShellPreflightService;
        this.workspaceInventoryService = workspaceInventoryService;
        this.workspaceReadActivity = workspaceReadActivity;
        this.workspaceReadActivity.ActivityReported += this.OnWorkspaceActivityReported;
        this.tqlQueryRunner = tqlQueryRunner;
        this.upgradeCopilotService = upgradeCopilotService;
        this.upgradeRiskScanner = upgradeRiskScanner;
        this.inventoryUpgradeRiskScanner = inventoryUpgradeRiskScanner;
        this.upgradeRiskReportGenerator = upgradeRiskReportGenerator;
        this.upgradeComparisonReportGenerator = upgradeComparisonReportGenerator;
        this.workspaceNavigatorService = workspaceNavigatorService;
        this.inventoryCopilotService = inventoryCopilotService;
        this.healthIssuesView = CollectionViewSource.GetDefaultView(this.HealthIssues);
        this.healthIssuesView.Filter = this.FilterHealthIssue;

        var preferences = this.userPreferencesService.Load();
        this.OutputFolder = preferences.OutputFolder;
        this.LoadExistingInventoryFromDefaultLocation();
        // This prototype intentionally supports one documented upgrade path only.
        this.SourceVersion = "2025.1";
        this.TargetVersion = "2026.1";

        var installation = this.installationService.GetInstallation();

        this.StatusMessage = installation.IsInstalled
            ? "Tosca installation detected."
            : "Tosca installation not found.";
    }

    // ====================================================
    // Browse Workspace
    // ====================================================
    [RelayCommand]
    private async Task BrowseWorkspace()
    {
        var dialog = new OpenFileDialog();

        dialog.Filter =
            "Tosca Workspace (*.tws)|*.tws";

        if (dialog.ShowDialog() != true)
        {
            return;
        }

        this.WorkspacePath = dialog.FileName;

        this.DetectWorkspace();
        await this.CheckTCShell();
    }

    // ====================================================
    // Browse Output Folder
    // ====================================================
    [RelayCommand]
    private void BrowseOutputFolder()
    {
        var dialog = new OpenFolderDialog();

        if (dialog.ShowDialog() == true)
        {
            this.OutputFolder = dialog.FolderName;
            this.SavePreferences();
        }
    }

    // ====================================================
    // Detect Workspace
    // ====================================================
    private void DetectWorkspace()
    {
        var info =
            this.workspaceDetector.Detect(this.WorkspacePath);

        this.RepositoryType = info.RepositoryType;

        this.ProjectId = info.ProjectId ?? string.Empty;

        this.Username = info.DefaultUser ?? string.Empty;

        this.ShowSqlAuthentication =
            info.RequiresUserPassword;

        this.ShowTsrAuthentication =
            info.RequiresClientSecret;

        this.WorkspaceAccessStatus = info.RequiresUserPassword
            ? "This database workspace requires a Tosca username and password before it can be read."
            : info.RequiresClientSecret
                ? "This managed workspace requires a client ID and client secret before it can be read."
                : "This workspace uses its local Tosca configuration. No separate credentials were found in the .tws file.";

        this.StatusMessage =
            $"Repository detected : {this.RepositoryType}";
    }

    // ====================================================
    // Analyze
    // ====================================================
    [RelayCommand]
    private async Task Analyze()
    {
        if (string.IsNullOrWhiteSpace(this.WorkspacePath))
        {
            this.StatusMessage = "Please select a workspace.";
            return;
        }

        this.IsAnalyzing = true;
        this.AnalysisProgress = 0;
        this.AnalysisProgressMessage = "Preparing analysis...";
        this.analysisCancellationSource = new CancellationTokenSource();

        try
        {
            var request = new WorkspaceRequest
            {
                WorkspacePath = this.WorkspacePath,
                Username = this.Username,
                Password = this.Password,
                ClientId = this.ClientId,
                ClientSecret = this.ClientSecret,
                IsManagedRepository = this.ShowTsrAuthentication,
                SourceVersion = this.SourceVersion,
                TargetVersion = this.TargetVersion,
                OutputFolder = this.OutputFolder,
            };

            var progress = new Progress<AnalysisProgress>(update =>
            {
                this.AnalysisProgress = update.Percentage;
                this.AnalysisProgressMessage = update.Message;
            });

            var result = await this.workspaceAnalyzer.AnalyzeAsync(
                request,
                progress,
                this.analysisCancellationSource.Token);

            this.StatusMessage = result.Success
                ? result.Message
                : "Workspace analysis failed.";

            if (result.Success)
            {
                this.lastAnalysisResult = result;
                this.TotalObjects = result.TotalObjects;
                this.HealthScore = result.HealthScore;
                this.HealthIssueCount = result.HealthIssueCount;
                this.BlockingIssues = result.BlockingIssues;
                this.Warnings = result.Warnings;
                this.UpgradeStatus = result.ReadyForUpgrade ? "Ready for upgrade" : "Needs attention";
                this.ReportPath = result.HtmlReportFile;
                this.HealthIssues.Clear();
                this.UpgradeRecommendations.Clear();

                foreach (var issue in result.HealthIssues)
                {
                    this.HealthIssues.Add(issue);
                }

                foreach (var recommendation in result.UpgradeRecommendations)
                {
                    this.UpgradeRecommendations.Add(recommendation);
                }

                this.healthIssuesView.Refresh();
                await this.CreateCopilotBriefingAsync();
            }
        }
        catch (OperationCanceledException)
        {
            this.StatusMessage = "Analysis was cancelled. No final report was generated.";
            this.AnalysisProgressMessage = "Cancelled";
        }
        catch (Exception ex)
        {
            this.StatusMessage = this.GetFriendlyErrorMessage(ex);
            this.AnalysisProgressMessage = "Failed";
        }
        finally
        {
            this.analysisCancellationSource?.Dispose();
            this.analysisCancellationSource = null;
            this.IsAnalyzing = false;
        }
    }

    [RelayCommand]
    private async Task CheckTCShell()
    {
        try
        {
            this.TcShellStatus = "Checking TCShell installation and Tosca license...";
            var result = await this.tcShellPreflightService.CheckAsync();
            this.TcShellReady = result.IsReady;
            this.TcShellStatus = result.Message;
        }
        catch (Exception exception)
        {
            this.TcShellReady = false;
            this.TcShellStatus = this.GetFriendlyErrorMessage(exception);
        }
    }

    [RelayCommand]
    private async Task AnalyzeWorkspaceInventory()
    {
        if (!this.TcShellReady)
        {
            this.WorkspaceInventoryStatus = "Verify TCShell and the Tosca license before analyzing the workspace.";
            return;
        }

        if (string.IsNullOrWhiteSpace(this.WorkspacePath))
        {
            this.WorkspaceInventoryStatus = "Select a Tosca workspace (.tws) before analyzing it.";
            return;
        }

        if (this.ShowSqlAuthentication && (string.IsNullOrWhiteSpace(this.Username) || string.IsNullOrWhiteSpace(this.Password)))
        {
            this.WorkspaceInventoryStatus = "Enter both the Tosca username and password required by this database workspace.";
            return;
        }

        if (this.ShowTsrAuthentication && (string.IsNullOrWhiteSpace(this.ClientId) || string.IsNullOrWhiteSpace(this.ClientSecret)))
        {
            this.WorkspaceInventoryStatus = "Enter both the client ID and client secret required by this managed workspace.";
            return;
        }

        this.IsWorkspaceInventoryAnalyzing = true;
        this.WorkspaceActivity.Clear();
        this.WorkspaceInventoryStatus = "Reading the workspace tree through TCShell...";
        this.workspaceInventoryCancellationSource = new CancellationTokenSource();

        try
        {
            var result = await this.workspaceInventoryService.AnalyzeAsync(new WorkspaceRequest
            {
                WorkspacePath = this.WorkspacePath,
                Username = this.Username,
                Password = this.Password,
                ClientId = this.ClientId,
                ClientSecret = this.ClientSecret,
                IsManagedRepository = this.ShowTsrAuthentication,
                SourceVersion = this.SourceVersion,
                TargetVersion = this.TargetVersion,
                OutputFolder = this.OutputFolder,
            }, this.workspaceInventoryCancellationSource.Token);

            this.WorkspaceInventoryObjectCount = result.ObjectCount;
            this.WorkspaceInventoryPath = result.InventoryFilePath;
            var mode = result.IsIncremental ? "Incremental" : "Full";
            this.WorkspaceInventoryStatus = $"{mode} inventory complete: {result.NewNodeCount:N0} new node(s), {result.ObjectCount:N0} total node(s) saved to JSON.";
        }
        catch (OperationCanceledException)
        {
            this.WorkspaceInventoryStatus = "Workspace inventory was cancelled.";
        }
        catch (Exception exception)
        {
            this.WorkspaceInventoryStatus = this.GetFriendlyErrorMessage(exception);
        }
        finally
        {
            this.workspaceInventoryCancellationSource?.Dispose();
            this.workspaceInventoryCancellationSource = null;
            this.IsWorkspaceInventoryAnalyzing = false;
        }
    }

    [RelayCommand]
    private void CancelWorkspaceInventory()
    {
        if (!this.IsWorkspaceInventoryAnalyzing || this.workspaceInventoryCancellationSource is null)
        {
            return;
        }

        this.WorkspaceInventoryStatus = "Cancelling the active TCShell batch...";
        this.workspaceInventoryCancellationSource.Cancel();
    }

    [RelayCommand]
    private async Task AskInventoryAi()
    {
        if (string.IsNullOrWhiteSpace(this.WorkspaceInventoryPath) || !File.Exists(this.WorkspaceInventoryPath))
        {
            this.InventoryAiAnswer = "Use an existing WorkspaceInventory.json or create one once first. AI reads only that saved JSON and never starts a scan.";
            return;
        }

        if (string.IsNullOrWhiteSpace(this.InventoryQuestion))
        {
            this.InventoryAiAnswer = "Enter a question, for example: Which modules are duplicate or unlinked from test cases?";
            return;
        }

        this.IsInventoryAiWorking = true;
        this.InventoryAiAnswer = "Reading relevant inventory evidence and preparing the answer...";
        try
        {
            var response = await this.inventoryCopilotService.AskAsync(this.WorkspaceInventoryPath, this.InventoryQuestion);
            this.InventoryAiAnswer = response.Answer;
        }
        catch (Exception exception)
        {
            this.InventoryAiAnswer = this.GetFriendlyErrorMessage(exception);
        }
        finally
        {
            this.IsInventoryAiWorking = false;
        }
    }

    [RelayCommand]
    private async Task UseExistingInventoryJson()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Workspace inventory JSON (*.json)|*.json",
            Title = "Select an existing WorkspaceInventory.json file",
        };
        if (dialog.ShowDialog() != true)
        {
            return;
        }

        try
        {
            var inventory = JsonSerializer.Deserialize<WorkspaceInventorySnapshot>(
                await File.ReadAllTextAsync(dialog.FileName),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (inventory is null || inventory.Nodes.Count == 0)
            {
                this.InventoryAiAnswer = "The selected file is not a usable workspace inventory JSON.";
                return;
            }

            this.WorkspaceInventoryPath = dialog.FileName;
            this.WorkspaceInventoryObjectCount = inventory.ObjectCount;
            this.WorkspaceInventoryStatus = $"Existing inventory loaded: {inventory.ObjectCount:N0} nodes. AI and rules will use this JSON only; no TCShell scan will run.";
            this.InventoryAiAnswer = $"Existing JSON loaded ({inventory.ObjectCount:N0} nodes). Ask anything about this inventory without scanning the workspace again.";
        }
        catch (Exception exception)
        {
            this.InventoryAiAnswer = this.GetFriendlyErrorMessage(exception);
        }
    }

    [RelayCommand]
    private async Task EvaluateInventoryUpgradeRules()
    {
        if (string.IsNullOrWhiteSpace(this.WorkspaceInventoryPath) || !File.Exists(this.WorkspaceInventoryPath))
        {
            this.InventoryUpgradeScanStatus = "Create a successful workspace inventory first. The JSON file is required for this offline rule evaluation.";
            return;
        }

        this.IsInventoryUpgradeScanning = true;
        this.InventoryUpgradeScanStatus = "Evaluating documented upgrade rules against the saved inventory JSON...";

        try
        {
            var inventoryResult = await this.inventoryUpgradeRiskScanner.ScanAsync(
                this.WorkspaceInventoryPath,
                this.SourceVersion,
                this.TargetVersion);
            var findings = inventoryResult.Findings.ToList();
            var diagnostics = inventoryResult.Diagnostics.ToList();

            if (this.IncludeExecutionHealth)
            {
                await this.AppendExecutionHealthFindingsAsync(findings, diagnostics);
            }

            var result = new UpgradeRiskScanResult
            {
                Findings = findings,
                Diagnostics = diagnostics,
            };

            this.UpgradeRiskFindings.Clear();
            foreach (var finding in result.Findings)
            {
                this.UpgradeRiskFindings.Add(finding);
            }

            var inventory = JsonSerializer.Deserialize<WorkspaceInventorySnapshot>(
                await File.ReadAllTextAsync(this.WorkspaceInventoryPath))
                ?? throw new InvalidDataException("The saved workspace inventory could not be read for report generation.");
            var outputFolder = Path.GetDirectoryName(this.WorkspaceInventoryPath)!;
            this.UpgradeRiskReportPath = Path.Combine(outputFolder, "UpgradeComparisonReport.html");
            await this.upgradeComparisonReportGenerator.GenerateAsync(inventory, result, this.UpgradeRiskReportPath);

            this.InventoryUpgradeScanStatus = result.Diagnostics.Count > 0
                ? result.Diagnostics[0]
                : $"Rule evaluation complete: {result.Findings.Count:N0} finding(s). Report created.";
        }
        catch (Exception exception)
        {
            this.InventoryUpgradeScanStatus = this.GetFriendlyErrorMessage(exception);
        }
        finally
        {
            this.IsInventoryUpgradeScanning = false;
        }
    }

    private async Task AppendExecutionHealthFindingsAsync(
        ICollection<UpgradeRiskFinding> findings,
        ICollection<string> diagnostics)
    {
        if (!this.TcShellReady)
        {
            diagnostics.Add("Execution-health check was skipped because TCShell license verification is not ready.");
            return;
        }

        const string recoveryQuery = ">SUBPARTS:ExecutionTestCaseLog[Recovered == \"True\"]=>SUPERPART:ExecutionList=>SUBPARTS:ExecutionEntry";
        var queryResult = await this.tqlQueryRunner.RunAsync(recoveryQuery, new WorkspaceRequest
        {
            WorkspacePath = this.WorkspacePath,
            Username = this.Username,
            Password = this.Password,
            ClientId = this.ClientId,
            ClientSecret = this.ClientSecret,
            IsManagedRepository = this.ShowTsrAuthentication,
        });

        if (!queryResult.Success)
        {
            diagnostics.Add($"Execution-health check could not run: {queryResult.Message}");
            return;
        }

        foreach (var item in queryResult.Document.Objects)
        {
            findings.Add(new UpgradeRiskFinding(
                "WG-EXEC-001",
                "Advisory",
                "Execution entry recovered by a recovery scenario",
                item.Name,
                item.ObjectType,
                "The execution log shows recovery. The test may pass while depending on a recovery path, so prioritize it for post-upgrade regression validation.",
                "Run this ExecutionEntry in the staging regression suite and investigate recurring recovery behavior.",
                $"TQL: {recoveryQuery}"));
        }
    }

    private void OnWorkspaceActivityReported(object? sender, WorkspaceReadActivityEventArgs eventArgs)
    {
        void AddEntry()
        {
            this.WorkspaceActivity.Add(new WorkspaceActivityEntry
            {
                Timestamp = eventArgs.Timestamp.ToString("HH:mm:ss"),
                Level = eventArgs.Level.ToString(),
                Message = eventArgs.Message,
            });
        }

        if (Application.Current.Dispatcher.CheckAccess())
        {
            AddEntry();
            return;
        }

        Application.Current.Dispatcher.Invoke(AddEntry);
    }

    [RelayCommand]
    private async Task RunTqlQuery()
    {
        if (!this.TcShellReady)
        {
            this.TqlQueryStatus = "Run Check TCShell successfully before executing a TQL query.";
            return;
        }

        if (string.IsNullOrWhiteSpace(this.WorkspacePath))
        {
            this.TqlQueryStatus = "Select a Tosca workspace before executing a TQL query.";
            return;
        }

        try
        {
            this.TqlQueryStatus = "Running read-only TQL query...";
            var result = await this.tqlQueryRunner.RunAsync(
                this.TqlQuery,
                new WorkspaceRequest
                {
                    WorkspacePath = this.WorkspacePath,
                    Username = this.Username,
                    Password = this.Password,
                    ClientId = this.ClientId,
                    ClientSecret = this.ClientSecret,
                    IsManagedRepository = this.ShowTsrAuthentication,
                });
            this.TqlResults.Clear();

            foreach (var item in result.Document.Objects)
            {
                this.TqlResults.Add(item);
            }

            this.TqlQueryStatus = result.Message;
        }
        catch (Exception exception)
        {
            this.TqlQueryStatus = this.GetFriendlyErrorMessage(exception);
        }
    }

    [RelayCommand]
    private async Task AskWorkspaceCopilot()
    {
        if (!this.TcShellReady)
        {
            this.NavigatorAnswer = "Run Check TCShell successfully before asking Copilot about the workspace.";
            return;
        }

        if (string.IsNullOrWhiteSpace(this.WorkspacePath) || string.IsNullOrWhiteSpace(this.NavigatorQuestion))
        {
            this.NavigatorAnswer = "Select a workspace and enter a question first.";
            return;
        }

        try
        {
            this.NavigatorAnswer = "Copilot is generating safe TQL and scanning the workspace...";
            var response = await this.workspaceNavigatorService.AskAsync(
                this.NavigatorQuestion,
                new WorkspaceRequest
                {
                    WorkspacePath = this.WorkspacePath,
                    Username = this.Username,
                    Password = this.Password,
                    ClientId = this.ClientId,
                    ClientSecret = this.ClientSecret,
                    IsManagedRepository = this.ShowTsrAuthentication,
                    SourceVersion = this.SourceVersion,
                    TargetVersion = this.TargetVersion,
                });
            this.NavigatorGeneratedTql = response.GeneratedTql;
            this.NavigatorAnswer = response.Answer;
            this.NavigatorResults.Clear();

            foreach (var item in response.Results.Objects)
            {
                this.NavigatorResults.Add(item);
            }
        }
        catch (Exception exception)
        {
            this.NavigatorAnswer = this.GetFriendlyErrorMessage(exception);
        }
    }

    [RelayCommand]
    private async Task ScanUpgradeRisks()
    {
        if (!this.TcShellReady)
        {
            this.UpgradeRiskScanStatus = "Run Check TCShell successfully before scanning upgrade risks.";
            return;
        }

        if (string.IsNullOrWhiteSpace(this.WorkspacePath))
        {
            this.UpgradeRiskScanStatus = "Select a Tosca workspace before scanning upgrade risks.";
            return;
        }

        try
        {
            this.UpgradeRiskScanStatus = "Running approved 2026.1 upgrade rules through TQL...";
            var result = await this.upgradeRiskScanner.ScanAsync(
                new WorkspaceRequest
                {
                    WorkspacePath = this.WorkspacePath,
                    Username = this.Username,
                    Password = this.Password,
                    ClientId = this.ClientId,
                    ClientSecret = this.ClientSecret,
                    IsManagedRepository = this.ShowTsrAuthentication,
                    SourceVersion = this.SourceVersion,
                    TargetVersion = this.TargetVersion,
                });
            this.UpgradeRiskFindings.Clear();
            foreach (var finding in result.Findings)
            {
                this.UpgradeRiskFindings.Add(finding);
            }

            this.lastAnalysisResult = CreateCopilotAnalysis(result.Findings);
            this.UpgradeRiskScanStatus = result.Diagnostics.Count == 0
                ? $"Upgrade risk scan complete: {result.Findings.Count} finding(s)."
                : $"Upgrade risk scan complete with {result.Diagnostics.Count} rule diagnostic(s).";
            await this.CreateCopilotBriefingAsync();
            var outputFolder = string.IsNullOrWhiteSpace(this.OutputFolder)
                ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ToscaUpgradeCopilot")
                : this.OutputFolder;
            this.UpgradeRiskReportPath = Path.Combine(outputFolder, "UpgradeRiskReport.html");
            await this.upgradeRiskReportGenerator.GenerateAsync(
                result,
                this.SourceVersion,
                this.TargetVersion,
                this.CopilotBriefing,
                this.UpgradeRiskReportPath);
        }
        catch (Exception exception)
        {
            this.UpgradeRiskScanStatus = this.GetFriendlyErrorMessage(exception);
        }
    }

    [RelayCommand]
    private async Task AskCopilot()
    {
        if (this.lastAnalysisResult is null)
        {
            this.CopilotAnswer = "Run an upgrade scan first so Copilot can use verified workspace evidence.";
            return;
        }

        if (string.IsNullOrWhiteSpace(this.CopilotQuestion))
        {
            this.CopilotAnswer = "Enter a support question about the scan findings.";
            return;
        }

        try
        {
            this.IsCopilotWorking = true;
            this.CopilotAnswer = "Copilot is reviewing verified upgrade evidence...";
            this.CopilotAnswer = await this.upgradeCopilotService.AnswerAsync(
                this.lastAnalysisResult,
                this.SourceVersion,
                this.TargetVersion,
                this.CopilotQuestion);
        }
        catch (Exception exception)
        {
            this.CopilotAnswer = $"Copilot could not respond: {exception.Message}";
        }
        finally
        {
            this.IsCopilotWorking = false;
        }
    }

    private async Task CreateCopilotBriefingAsync()
    {
        if (this.lastAnalysisResult is null)
        {
            return;
        }

        try
        {
            this.IsCopilotWorking = true;
            this.CopilotBriefing = "Copilot is preparing the support briefing...";
            this.CopilotBriefing = await this.upgradeCopilotService.CreateBriefingAsync(
                this.lastAnalysisResult,
                this.SourceVersion,
                this.TargetVersion);
        }
        catch (Exception exception)
        {
            this.CopilotBriefing = $"Copilot briefing unavailable: {exception.Message}";
        }
        finally
        {
            this.IsCopilotWorking = false;
        }
    }

    private static AnalysisResult CreateCopilotAnalysis(IReadOnlyCollection<UpgradeRiskFinding> findings)
    {
        return new AnalysisResult
        {
            Success = true,
            TotalObjects = findings.Count,
            HealthIssueCount = findings.Count,
            BlockingIssues = findings.Count(finding => finding.Severity == "Blocker"),
            Warnings = findings.Count(finding => finding.Severity == "Warning"),
            HealthIssues = findings.Select(finding => new HealthIssue
            {
                RuleId = finding.RuleId,
                Severity = finding.Severity,
                Title = finding.Title,
                ObjectName = finding.ObjectName,
                NodePath = finding.TqlQuery,
                Description = finding.Description,
            }).ToList(),
            UpgradeRecommendations = findings.Select(finding => finding.RecommendedAction).Distinct().ToList(),
        };
    }

    [RelayCommand]
    private void BrowseBaselineSnapshot()
    {
        this.BrowseSnapshot(path => this.BaselineSnapshotPath = path);
    }

    [RelayCommand]
    private void BrowseSubset()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Tosca subset (*.tsu)|*.tsu",
        };

        if (dialog.ShowDialog() == true)
        {
            this.SubsetPath = dialog.FileName;
        }
    }

    [RelayCommand]
    private async Task AnalyzeSubset()
    {
        if (!File.Exists(this.SubsetPath))
        {
            this.SubsetStatus = "Select a valid Tosca subset (.tsu) file.";
            return;
        }

        try
        {
            this.SubsetStatus = "Reading subset and checking upgrade rules...";
            var result = await this.subsetAnalyzer.AnalyzeAsync(this.SubsetPath, this.SourceVersion, this.TargetVersion);
            this.SubsetProjectName = result.ProjectName;
            this.SubsetEntityCount = result.EntityCount;
            this.SubsetFindings.Clear();

            foreach (var finding in result.Findings)
            {
                this.SubsetFindings.Add(finding);
            }

            this.SubsetStatus = $"Subset analysis complete: {result.EntityCount:N0} entities, {result.Findings.Count} upgrade consideration(s).";
        }
        catch (InvalidDataException)
        {
            this.SubsetStatus = "The selected file is not a readable Tosca subset (.tsu).";
        }
        catch (Exception exception)
        {
            this.SubsetStatus = this.GetFriendlyErrorMessage(exception);
        }
    }

    [RelayCommand]
    private void BrowseRepositoryDatabase()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Tosca repository database (*.db)|*.db|All files (*.*)|*.*",
        };

        if (dialog.ShowDialog() == true)
        {
            this.RepositoryDatabasePath = dialog.FileName;
        }
    }

    [RelayCommand]
    private async Task AnalyzeRepositoryDatabase()
    {
        if (!File.Exists(this.RepositoryDatabasePath))
        {
            this.RepositoryDatabaseStatus = "Select a valid local Tosca repository database (.db).";
            return;
        }

        try
        {
            this.RepositoryDatabaseStatus = "Opening the database in read-only mode...";
            var result = await this.repositoryDatabaseAnalyzer.AnalyzeAsync(
                this.RepositoryDatabasePath,
                this.SourceVersion,
                this.TargetVersion);
            this.RepositoryDatabaseObjectCount = result.TotalObjects;
            this.RepositoryDatabaseTypeCount = result.ObjectTypes;
            this.RepositoryTypeCounts.Clear();
            this.RepositoryDatabaseFindings.Clear();

            foreach (var typeCount in result.TypeCounts)
            {
                this.RepositoryTypeCounts.Add(typeCount);
            }

            foreach (var finding in result.Findings)
            {
                this.RepositoryDatabaseFindings.Add(finding);
            }

            this.RepositoryDatabaseStatus = $"Repository inventory complete: {result.TotalObjects:N0} objects across {result.ObjectTypes:N0} types. The database was read only.";
        }
        catch (SqliteException)
        {
            this.RepositoryDatabaseStatus = "This file is not a supported Tosca local repository database, or it is currently unavailable.";
        }
        catch (Exception exception)
        {
            this.RepositoryDatabaseStatus = this.GetFriendlyErrorMessage(exception);
        }
    }

    [RelayCommand]
    private void BrowseCurrentSnapshot()
    {
        this.BrowseSnapshot(path => this.CurrentSnapshotPath = path);
    }

    [RelayCommand]
    private void CancelAnalysis()
    {
        if (!this.IsAnalyzing || this.analysisCancellationSource is null)
        {
            return;
        }

        this.AnalysisProgressMessage = "Cancelling analysis...";
        this.analysisCancellationSource.Cancel();
    }

    [RelayCommand]
    private async Task CompareSnapshots()
    {
        if (!File.Exists(this.BaselineSnapshotPath) || !File.Exists(this.CurrentSnapshotPath))
        {
            this.CompareStatus = "Select valid baseline and current Snapshot.json files.";
            return;
        }

        try
        {
            this.CompareStatus = "Comparing snapshots...";
            var result = await this.workspaceCompareService.CompareAsync(this.BaselineSnapshotPath, this.CurrentSnapshotPath);
            this.AddedObjectCount = result.AddedObjects.Count;
            this.RemovedObjectCount = result.RemovedObjects.Count;
            this.ChangedPropertyCount = result.PropertyChanges.Count;

            var outputFolder = string.IsNullOrWhiteSpace(this.OutputFolder)
                ? Path.GetDirectoryName(this.CurrentSnapshotPath)!
                : this.OutputFolder;
            var jsonReport = Path.Combine(outputFolder, "SnapshotCompare.json");
            var htmlReport = Path.Combine(outputFolder, "SnapshotCompare.html");
            await this.compareReportExporter.ExportAsync(result, jsonReport);
            await this.compareHtmlReportGenerator.GenerateAsync(result, htmlReport);

            this.CompareReportPath = htmlReport;
            this.CompareStatus = "Comparison complete.";
        }
        catch (Exception exception)
        {
            this.CompareStatus = this.GetFriendlyErrorMessage(exception);
        }
    }

    private string GetFriendlyErrorMessage(Exception exception)
    {
        return exception switch
        {
            DirectoryNotFoundException => "The selected output folder does not exist or cannot be accessed.",
            UnauthorizedAccessException => "Workspace Guardian does not have permission to read the workspace or write reports to the selected folder.",
            FileNotFoundException => "A required Tosca file was not found. Verify the workspace path and Tosca installation.",
            _ => $"Analysis could not be completed: {exception.Message}",
        };
    }

    partial void OnIssueSearchTextChanged(string value)
    {
        this.healthIssuesView.Refresh();
    }

    partial void OnSelectedSeverityChanged(string value)
    {
        this.healthIssuesView.Refresh();
    }

    private bool FilterHealthIssue(object item)
    {
        if (item is not HealthIssue issue)
        {
            return false;
        }

        var matchesSeverity = this.SelectedSeverity == "All" || issue.Severity == this.SelectedSeverity;

        if (!matchesSeverity)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(this.IssueSearchText))
        {
            return true;
        }

        var search = this.IssueSearchText.Trim();
        return issue.RuleId.Contains(search, StringComparison.OrdinalIgnoreCase)
            || issue.Title.Contains(search, StringComparison.OrdinalIgnoreCase)
            || issue.ObjectName.Contains(search, StringComparison.OrdinalIgnoreCase)
            || issue.NodePath.Contains(search, StringComparison.OrdinalIgnoreCase)
            || issue.Description.Contains(search, StringComparison.OrdinalIgnoreCase);
    }

    private void BrowseSnapshot(Action<string> setPath)
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Workspace Snapshot (Snapshot.json)|Snapshot.json|JSON files (*.json)|*.json",
        };

        if (dialog.ShowDialog() == true)
        {
            setPath(dialog.FileName);
        }
    }

    private void LoadExistingInventoryFromDefaultLocation()
    {
        var inventoryFolder = string.IsNullOrWhiteSpace(this.OutputFolder)
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ToscaUpgradeGuide")
            : this.OutputFolder;
        var inventoryPath = Path.Combine(inventoryFolder, "WorkspaceInventory.json");
        if (!File.Exists(inventoryPath))
        {
            return;
        }

        try
        {
            var inventory = JsonSerializer.Deserialize<WorkspaceInventorySnapshot>(
                File.ReadAllText(inventoryPath),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (inventory is null || inventory.Nodes.Count == 0)
            {
                return;
            }

            this.WorkspaceInventoryPath = inventoryPath;
            this.WorkspaceInventoryObjectCount = inventory.ObjectCount;
            this.WorkspaceInventoryStatus = $"Existing inventory ready: {inventory.ObjectCount:N0} nodes loaded for offline AI and rule analysis.";
        }
        catch (IOException)
        {
            // The inventory remains optional; the user can select a JSON file manually.
        }
        catch (JsonException)
        {
            // Ignore an invalid default JSON file and allow manual selection instead.
        }
    }

    [RelayCommand]
    private void OpenAnalysisReport()
    {
        this.OpenReport(this.ReportPath, "analysis report");
    }

    [RelayCommand]
    private void OpenUpgradeRiskReport()
    {
        this.OpenReport(this.UpgradeRiskReportPath, "upgrade risk report");
    }

    [RelayCommand]
    private void OpenCompareReport()
    {
        this.OpenReport(this.CompareReportPath, "comparison report");
    }

    private void OpenReport(string reportPath, string reportName)
    {
        if (!File.Exists(reportPath))
        {
            this.StatusMessage = $"The {reportName} has not been generated yet.";
            return;
        }

        Process.Start(new ProcessStartInfo(reportPath) { UseShellExecute = true });
    }

    [RelayCommand]
    private void SavePreferences()
    {
        try
        {
            this.userPreferencesService.Save(new UserPreferences
            {
                OutputFolder = this.OutputFolder,
                SourceVersion = this.SourceVersion,
                TargetVersion = this.TargetVersion,
            });
            this.PreferencesStatus = "Preferences saved locally.";
        }
        catch (Exception exception)
        {
            this.PreferencesStatus = $"Could not save preferences: {exception.Message}";
        }
    }
}

