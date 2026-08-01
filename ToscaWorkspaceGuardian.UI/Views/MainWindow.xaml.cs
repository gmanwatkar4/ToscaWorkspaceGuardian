using System.Windows;
using ToscaWorkspaceGuardian.UI.ViewModels;

namespace ToscaWorkspaceGuardian.UI.Views;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;
    }
}