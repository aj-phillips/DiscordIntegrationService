using Microsoft.Win32;

namespace DiscordIntegrationService.Core.Utils;

public static class StartupHelper
{
    private const string RUN_KEY = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
    private const string APP_NAME = "DiscordIntegrationService";

    public static void SetStartup(bool enable, string exePath)
    {
        if (!OperatingSystem.IsWindows()) return;

        using var key = Registry.CurrentUser.OpenSubKey(RUN_KEY, true);

        if (enable)
            key.SetValue(APP_NAME, $"\"{exePath}\"");
        else
            key.DeleteValue(APP_NAME, false);
    }

    public static bool IsStartupEnabled()
    {
        if (!OperatingSystem.IsWindows()) return false;

        using var key = Registry.CurrentUser.OpenSubKey(RUN_KEY, false);
        return key.GetValue(APP_NAME) != null;
    }
}