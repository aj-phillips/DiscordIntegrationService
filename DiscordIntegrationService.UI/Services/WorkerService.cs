using System.IO;
using DiscordIntegrationService.Core.Interfaces;
using DiscordIntegrationService.Core.Models;
using Microsoft.Extensions.Hosting;

namespace DiscordIntegrationService.UI.Services;

public class WorkerService : BackgroundService
{
    private readonly IPresenceUpdateDecisionService _decisionService;
    private readonly IDiscordPresenceUpdaterService _discordPresenceUpdaterService;
    private readonly ISettingsService _settingsService;
    private readonly IWindowMonitorService _windowMonitorService;

    private WindowInfo _lastWindow;
    private Settings _settings;

    public WorkerService(
        IWindowMonitorService windowMonitorService,
        IDiscordPresenceUpdaterService discordPresenceUpdaterService,
        ISettingsService settingsService,
        IPresenceUpdateDecisionService decisionService)
    {
        _windowMonitorService = windowMonitorService;
        _discordPresenceUpdaterService = discordPresenceUpdaterService;
        _settingsService = settingsService;

        _decisionService = decisionService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _settings = _settingsService.Load();

        var watcher = new FileSystemWatcher
        {
            Path = Path.GetDirectoryName(_settingsService.FilePath) ?? string.Empty,
            Filter = Path.GetFileName(_settingsService.FilePath),
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size
        };

        watcher.Changed += (_, __) =>
        {
            try
            {
                Thread.Sleep(200);
                _settings = _settingsService.Load();
                Console.WriteLine("[Worker] Settings reloaded.");

                _discordPresenceUpdaterService.TryReconnect(_settings);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Worker] Failed to reload settings: {ex.Message}");
            }
        };

        watcher.EnableRaisingEvents = true;

        await RunLoop(stoppingToken);
    }

    private async Task RunLoop(CancellationToken stoppingToken)
    {
        var currentInterval = _settings.PollIntervalSeconds;
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(currentInterval));

        while (!stoppingToken.IsCancellationRequested)
        {
            if (!await timer.WaitForNextTickAsync(stoppingToken))
                break;

            if (!_settings.EnableRichPresence)
            {
                Console.WriteLine("[Worker] Rich Presence disabled, clearing presence...");
                await _discordPresenceUpdaterService.ClearPresenceAsync();
                continue;
            }

            if (_settings.PollIntervalSeconds != currentInterval)
            {
                Console.WriteLine(
                    $"[Worker] PollIntervalSeconds changed from {currentInterval} to {_settings.PollIntervalSeconds}. Recreating timer...");
                currentInterval = _settings.PollIntervalSeconds;

                timer.Dispose();
                timer = new PeriodicTimer(TimeSpan.FromSeconds(currentInterval));
                continue;
            }

            var window = _windowMonitorService.GetActiveWindow();

            if (!ShouldUpdatePresence(window, _lastWindow, _settings))
                continue;

            await _discordPresenceUpdaterService.UpdatePresenceAsync(window, stoppingToken);
            _lastWindow = window;
        }
    }

    public bool ShouldUpdatePresence(WindowInfo window, WindowInfo lastWindow, Settings settings)
    {
        return _decisionService.ShouldUpdate(window, lastWindow, settings);
    }
}