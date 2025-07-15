using DiscordIntegrationService.Core.Models;

namespace DiscordIntegrationService.Core.Interfaces;

public interface IPresenceUpdateDecisionService
{
    public bool ShouldUpdate(WindowInfo? window, WindowInfo? lastWindow, Settings settings);
}