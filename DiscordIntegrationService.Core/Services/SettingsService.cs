using System.Text.Json;
using DiscordIntegrationService.Core.Interfaces;
using DiscordIntegrationService.Core.Models;

namespace DiscordIntegrationService.Core.Services;

public class SettingsService : ISettingsService
{
    public SettingsService(string filePath)
    {
        FilePath = filePath;
    }

    public string FilePath { get; }

    public Settings Load()
    {
        if (!File.Exists(FilePath))
        {
            // Create default settings
            var defaults = GetDefaultSettings();
            Save(defaults);

            Console.WriteLine($"[SettingsService] Created default settings at {FilePath}");
            return defaults;
        }

        var json = File.ReadAllText(FilePath);

        // Defensive fallback in case file is empty or corrupt
        if (string.IsNullOrWhiteSpace(json))
        {
            var defaults = GetDefaultSettings();
            Save(defaults);
            Console.WriteLine($"[SettingsService] Detected empty config. Rewrote defaults to {FilePath}");
            return defaults;
        }

        try
        {
            var settings = JsonSerializer.Deserialize<Settings>(json);
            return settings ?? GetDefaultSettings();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SettingsService] Failed to parse settings: {ex.Message}");
            // Optional: Backup the bad file
            File.Copy(FilePath, FilePath + ".bak", true);

            var defaults = GetDefaultSettings();
            Save(defaults);
            return defaults;
        }
    }

    public void Save(Settings settings)
    {
        var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(FilePath, json);
    }

    private static Settings GetDefaultSettings()
    {
        return new Settings
        {
            EnableRichPresence = false,
            PollIntervalSeconds = 2,
            ExcludeList = new List<string>(),
            DiscordClientId = "",
            RunOnStartup = false,
            PresenceDetailsTemplate = "{ProcessName}",
            PresenceStateTemplate = "{Title}"
        };
    }
}