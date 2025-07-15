using DiscordRPC;

namespace DiscordIntegrationService.Core.Interfaces;

public interface IDiscordRpcClient : IDisposable
{
    void Initialize();
    bool IsInitialized { get; }
    void SetPresence(RichPresence presence);
    void ClearPresence();
}