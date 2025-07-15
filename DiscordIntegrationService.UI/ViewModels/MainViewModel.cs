using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DiscordIntegrationService.Core.Interfaces;
using DiscordIntegrationService.Core.Models;
using DiscordIntegrationService.Core.Utils;

namespace DiscordIntegrationService.UI.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IDiscordPresenceUpdaterService _discordPresenceUpdaterService;
    private readonly ISettingsService _settingsService;

    [ObservableProperty] private string _discordClientId;

    [ObservableProperty] private bool _enableRichPresence;

    [ObservableProperty] private ObservableCollection<string> _excludeList;

    [ObservableProperty] private bool _isPasswordVisible;

    [ObservableProperty] private string _newExcludeItem;

    [ObservableProperty] private int _pollIntervalSeconds;

    [ObservableProperty] private string _presenceDetailsTemplate;

    [ObservableProperty] private string _presenceStateTemplate;

    [ObservableProperty] private bool _runOnStartup;

    [ObservableProperty] private string _selectedExcludeItem;

    public MainViewModel(ISettingsService settingsService, IDiscordPresenceUpdaterService discordPresenceUpdaterService)
    {
        _settingsService = settingsService;
        _discordPresenceUpdaterService = discordPresenceUpdaterService;

        var settings = _settingsService.Load();

        PopulateSettingsValues(settings);
    }

    partial void OnNewExcludeItemChanged(string value)
    {
        AddToExcludeListCommand.NotifyCanExecuteChanged();
    }

    partial void OnSelectedExcludeItemChanged(string value)
    {
        RemoveFromExcludeListCommand.NotifyCanExecuteChanged();
    }

    private static void RestartApplicationOnClientIdChange(string value, Settings settings)
    {
        if (settings.DiscordClientId == value) return;

        var result = MessageBox.Show(
            "Your Discord Application ID has changed.\nYou need to restart the application for changes to take effect.\n\nRestart now?",
            "Restart Required",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes) RestartApplication();
    }

    private void PopulateSettingsValues(Settings settings)
    {
        EnableRichPresence = settings.EnableRichPresence;

        ExcludeList = new ObservableCollection<string>(settings.ExcludeList);

        if (!string.IsNullOrWhiteSpace(settings.DiscordClientId))
            DiscordClientId = CryptoHelper.Deobfuscate(settings.DiscordClientId);

        PollIntervalSeconds = settings.PollIntervalSeconds;

        RunOnStartup = settings.RunOnStartup;

        PresenceDetailsTemplate = settings.PresenceDetailsTemplate ?? "{ProcessName}";
        PresenceStateTemplate = settings.PresenceStateTemplate ?? "{Title}";

        IsPasswordVisible = false;
    }

    private static void RestartApplication()
    {
        var exePath = Process.GetCurrentProcess().MainModule?.FileName;

        if (!string.IsNullOrEmpty(exePath)) Process.Start(exePath);

        Application.Current.Shutdown();
    }

    [RelayCommand(CanExecute = nameof(CanAdd))]
    private void AddToExcludeList()
    {
        if (!string.IsNullOrWhiteSpace(NewExcludeItem) && !ExcludeList.Contains(NewExcludeItem.Trim()))
        {
            ExcludeList.Add(NewExcludeItem.Trim());
            NewExcludeItem = string.Empty; // clear input
        }
    }

    private bool CanAdd()
    {
        return !string.IsNullOrWhiteSpace(NewExcludeItem) && !ExcludeList.Contains(NewExcludeItem.Trim());
    }

    [RelayCommand(CanExecute = nameof(CanRemove))]
    private void RemoveFromExcludeList()
    {
        if (!string.IsNullOrWhiteSpace(SelectedExcludeItem))
        {
            ExcludeList.Remove(SelectedExcludeItem);
            SelectedExcludeItem = null;
        }
    }

    private bool CanRemove()
    {
        return !string.IsNullOrWhiteSpace(SelectedExcludeItem);
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        var settings = new Settings
        {
            EnableRichPresence = EnableRichPresence,
            DiscordClientId = CryptoHelper.Obfuscate(DiscordClientId),
            PollIntervalSeconds = PollIntervalSeconds,
            ExcludeList = ExcludeList.ToList(),
            RunOnStartup = RunOnStartup,
            PresenceDetailsTemplate = PresenceDetailsTemplate,
            PresenceStateTemplate = PresenceStateTemplate
        };

        _settingsService.Save(settings);

        RestartApplicationOnClientIdChange(DiscordClientId, settings);

        if (!EnableRichPresence)
            await _discordPresenceUpdaterService.ClearPresenceAsync();
        else
            await _discordPresenceUpdaterService.ForceUpdateAsync();

        var exePath = Process.GetCurrentProcess().MainModule?.FileName;

        if (exePath != null)
            StartupHelper.SetStartup(RunOnStartup, exePath);
    }

    [RelayCommand]
    private void TogglePasswordVisibility()
    {
        IsPasswordVisible = !IsPasswordVisible;
    }
}