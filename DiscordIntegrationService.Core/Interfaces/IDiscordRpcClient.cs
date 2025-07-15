using DiscordRPC;

namespace DiscordIntegrationService.Core.Interfaces;

public interface IDiscordRpcClient : IDisposable
{
    bool IsInitialized { get; }
    void Initialize();
    void SetPresence(RichPresence presence);
    void ClearPresence();
}