using System.ComponentModel;
using System.Windows;
using DiscordIntegrationService.UI.ViewModels;

namespace DiscordIntegrationService.UI;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        base.OnClosing(e);

        e.Cancel = true;

        Hide();
    }
}