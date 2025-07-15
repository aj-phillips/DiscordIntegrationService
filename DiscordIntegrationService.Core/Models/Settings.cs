namespace DiscordIntegrationService.Core.Models;

public class Settings
{
    public bool EnableRichPresence { get; set; } = true;
    public string DiscordClientId { get; set; } = "";
    public List<string> ExcludeList { get; set; } = [];
    public int PollIntervalSeconds { get; set; } = 2;
    public bool RunOnStartup { get; set; } = false;
    public string PresenceDetailsTemplate { get; set; } = "{ProcessName}";
    public string PresenceStateTemplate { get; set; } = "{Title}";
}