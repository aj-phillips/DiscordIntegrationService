using DiscordIntegrationService.Core.Interfaces;
using DiscordIntegrationService.Core.Models;
using DiscordIntegrationService.Core.Utils;
using DiscordRPC;

namespace DiscordIntegrationService.Core.Services;

public sealed class DiscordPresenceUpdaterService : IDiscordPresenceUpdaterService, IDisposable
{
    private readonly ISettingsService _settingsService;
    private readonly IWindowMonitorService _windowMonitorService;
    private IDiscordRpcClient? _client;
    private string? _currentClientId;

    private bool _disposed;
    private bool _isPresenceSet;
    private Settings _settings;

    public DiscordPresenceUpdaterService(ISettingsService settingsService, IWindowMonitorService windowMonitorService,
        IDiscordRpcClient discordRpcClient)
    {
        _windowMonitorService = windowMonitorService;
        _settingsService = settingsService;
        _client = discordRpcClient;

        ReloadSettings();

        TryInitialize(_settings);
    }

    public void TryReconnect(Settings settings)
    {
        if (string.IsNullOrWhiteSpace(settings.DiscordClientId))
        {
            Console.WriteLine("[DiscordPresenceUpdaterService] No Client ID provided. Cannot reconnect.");
            return;
        }

        if (CryptoHelper.Deobfuscate(settings.DiscordClientId) == _currentClientId) return;

        Console.WriteLine(
            $"[DiscordPresenceUpdaterService] Reconnecting Discord RPC with new Client ID: {settings.DiscordClientId}");

        _client?.Dispose();
        TryInitialize(settings);
    }

    public Task UpdatePresenceAsync(WindowInfo window, CancellationToken token)
    {
        if (_client is not { IsInitialized: true }) return Task.CompletedTask;

        var details = _settings.PresenceDetailsTemplate
            .Replace("{ProcessName}", window.ProcessName)
            .Replace("{Title}", window.Title);

        var state = _settings.PresenceStateTemplate
            .Replace("{ProcessName}", window.ProcessName)
            .Replace("{Title}", window.Title);

        _client.SetPresence(new RichPresence
        {
            Details = details,
            State = state,
            Timestamps = Timestamps.Now
        });

        _isPresenceSet = true;

        return Task.CompletedTask;
    }

    public Task ClearPresenceAsync()
    {
        if (_client is not { IsInitialized: true })
            return Task.CompletedTask;

        if (_isPresenceSet)
        {
            _client.ClearPresence();
            _isPresenceSet = false;
        }

        return Task.CompletedTask;
    }

    public async Task ForceUpdateAsync()
    {
        ReloadSettings();
        var window = _windowMonitorService.GetActiveWindow();

        await UpdatePresenceAsync(window, CancellationToken.None);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            _client?.Dispose();
            _client = null;
        }

        _disposed = true;
    }

    private void ReloadSettings()
    {
        _settings = _settingsService.Load();
    }

    private void TryInitialize(Settings settings)
    {
        if (string.IsNullOrWhiteSpace(settings.DiscordClientId))
        {
            Console.WriteLine("[DiscordPresenceUpdaterService] No Client ID provided. Skipping init.");
            return;
        }

        var realClientId = CryptoHelper.Deobfuscate(settings.DiscordClientId);

        _client = new DiscordRpcClientAdapterService(realClientId);
        _client.Initialize();
        _currentClientId = realClientId;

        Console.WriteLine(
            $"[DiscordPresenceUpdaterService] Initialized Discord RPC with Client ID: {settings.DiscordClientId}");
    }
}