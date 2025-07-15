using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using DiscordIntegrationService.Core.Interfaces;
using DiscordIntegrationService.Core.Services;
using DiscordIntegrationService.Core.Utils;
using DiscordIntegrationService.UI.Services;
using DiscordIntegrationService.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace DiscordIntegrationService.UI;

public partial class App : Application
{
    private IHost _host;
    private NotifyIcon _notifyIcon;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        ShutdownMode = ShutdownMode.OnExplicitShutdown;

        _host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddSingleton<ISettingsService>(_ =>
                    new SettingsService(Path.Combine(AppContext.BaseDirectory, "settings.json")));
                services.AddSingleton<IWindowMonitorService, WindowMonitorService>();
                services.AddSingleton<IPresenceUpdateDecisionService, PresenceUpdateDecisionService>();

                services.AddSingleton<IDiscordRpcClient>(sp =>
                {
                    var settings = sp.GetRequiredService<ISettingsService>().Load();
                    var clientId = CryptoHelper.Deobfuscate(settings.DiscordClientId);
                    var client = new DiscordRpcClientAdapterService(clientId);

                    if (!string.IsNullOrWhiteSpace(clientId))
                        client.Initialize();
                    else
                        Console.WriteLine("[DI] No Discord Client ID provided. Skipping Discord RPC init.");

                    return client;
                });

                services.AddSingleton<IDiscordPresenceUpdaterService, DiscordPresenceUpdaterService>();

                services.AddSingleton<MainViewModel>();
                services.AddSingleton<MainWindow>();

                services.AddHostedService<WorkerService>();
            })
            .Build();

        _host.Start();

        var iconPath = Path.Combine(AppContext.BaseDirectory, "Resources", "dis-logo.ico");

        if (!File.Exists(iconPath)) MessageBox.Show($"Icon not found: {iconPath}");

        _notifyIcon = new NotifyIcon
        {
            Icon = new Icon(iconPath),
            Visible = true,
            Text = "Discord Integration Service",
            ContextMenuStrip = new ContextMenuStrip()
        };

        _notifyIcon.ContextMenuStrip.Items.Add("Open Settings", null, (_, __) => ShowSettings());
        _notifyIcon.ContextMenuStrip.Items.Add("Exit", null, (_, __) => ExitApp());
    }

    private void ShowSettings()
    {
        var window = _host.Services.GetRequiredService<MainWindow>();

        if (!window.IsVisible)
            window.Show();
        else
            window.Activate();
    }

    private void ExitApp()
    {
        _notifyIcon.Visible = false;
        _host.Dispose();
        Shutdown();
    }
}