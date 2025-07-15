using DiscordIntegrationService.Core.Models;

namespace DiscordIntegrationService.Core.Interfaces;

public interface IWindowMonitorService
{
    WindowInfo GetActiveWindow();
}