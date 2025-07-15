using DiscordIntegrationService.Core.Interfaces;
using DiscordIntegrationService.Core.Models;

namespace DiscordIntegrationService.Core.Services;

public class PresenceUpdateDecisionService : IPresenceUpdateDecisionService
{
    public bool ShouldUpdate(WindowInfo? window, WindowInfo? lastWindow, Settings settings)
    {
        if (window == null) return false;

        if (lastWindow != null &&
            window.Title == lastWindow.Title &&
            window.ProcessName == lastWindow.ProcessName)
            return false;

        return !settings.ExcludeList.Contains(window.ProcessName, StringComparer.OrdinalIgnoreCase);
    }
}