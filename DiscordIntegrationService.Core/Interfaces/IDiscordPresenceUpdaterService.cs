using DiscordIntegrationService.Core.Models;

namespace DiscordIntegrationService.Core.Interfaces;

public interface IDiscordPresenceUpdaterService
{
    Task UpdatePresenceAsync(WindowInfo window, CancellationToken token);
    public void TryReconnect(Settings settings);
    public Task ForceUpdateAsync();
    public Task ClearPresenceAsync();
}