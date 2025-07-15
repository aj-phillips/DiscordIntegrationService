using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using DiscordIntegrationService.Core.Interfaces;
using DiscordIntegrationService.Core.Models;

namespace DiscordIntegrationService.Core.Services;

public class WindowMonitorService : IWindowMonitorService
{
    public WindowInfo GetActiveWindow()
    {
        const int nChars = 256;
        var buffer = new StringBuilder(nChars);
        var handle = GetForegroundWindow();

        if (GetWindowText(handle, buffer, nChars) <= 0) return null;

        GetWindowThreadProcessId(handle, out var lpdwProcessId);

        var proc = Process.GetProcessById(lpdwProcessId);

        return new WindowInfo
        {
            Title = buffer.ToString(),
            ProcessName = proc.ProcessName
        };
    }

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

    [DllImport("user32.dll")]
    private static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);
}