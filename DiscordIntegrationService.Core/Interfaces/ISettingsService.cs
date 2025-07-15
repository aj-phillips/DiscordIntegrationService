using DiscordIntegrationService.Core.Models;

namespace DiscordIntegrationService.Core.Interfaces;

public interface ISettingsService
{
    string FilePath { get; }
    Settings Load();
    void Save(Settings settings);
}