using DiscordIntegrationService.Core.Interfaces;
using DiscordRPC;

namespace DiscordIntegrationService.Core.Services;

public class DiscordRpcClientAdapterService : IDiscordRpcClient
{
    private readonly DiscordRpcClient _client;

    private bool _disposed;

    public DiscordRpcClientAdapterService(string clientId)
    {
        _client = new DiscordRpcClient(clientId);
    }

    public void Initialize() => _client.Initialize();

    public bool IsInitialized => _client.IsInitialized;

    public void SetPresence(RichPresence presence) => _client.SetPresence(presence);

    public void ClearPresence() => _client.ClearPresence();
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            _client?.Dispose();
        }

        _disposed = true;
    }
}