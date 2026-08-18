namespace ToscaWorkspaceGuardian.UI.Views;

using System.Windows;
using ToscaWorkspaceGuardian.UI.ViewModels;

public partial class ActivityWindow : Window
{
    public ActivityWindow(MainViewModel viewModel)
    {
        this.InitializeComponent();
        this.DataContext = viewModel;
    }
}
