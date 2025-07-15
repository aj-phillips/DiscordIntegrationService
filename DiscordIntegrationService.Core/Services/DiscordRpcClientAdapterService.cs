using DiscordIntegrationService.Core.Interfaces;
using DiscordRPC;

namespace DiscordIntegrationService.Core.Services;

public class DiscordRpcClientAdapterService : IDiscordRpcClient
{
    private readonly string? _clientId;
    private DiscordRpcClient _client;

    private bool _disposed;

    public DiscordRpcClientAdapterService(string? clientId)
    {
        _clientId = clientId;
    }

    public void Initialize()
    {
        if (string.IsNullOrWhiteSpace(_clientId))
        {
            Console.WriteLine("[DiscordRpcClientAdapterService] Client ID is empty. Skipping initialization.");
            return;
        }

        if (_client is { IsInitialized: true })
        {
            Console.WriteLine("[DiscordRpcClientAdapterService] Already initialized.");
            return;
        }

        _client = new DiscordRpcClient(_clientId);
        _client.Initialize();
    }

    public bool IsInitialized => _client?.IsInitialized ?? false;

    public void SetPresence(RichPresence presence)
    {
        if (!IsInitialized)
        {
            Console.WriteLine("[DiscordRpcClientAdapterService] Not initialized. Skipping presence update.");
            return;
        }

        _client!.SetPresence(presence);
    }

    public void ClearPresence()
    {
        if (!IsInitialized)
        {
            Console.WriteLine("[DiscordRpcClientAdapterService] Not initialized. Skipping clear.");
            return;
        }

        _client!.ClearPresence();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing) _client?.Dispose();

        _disposed = true;
    }
}