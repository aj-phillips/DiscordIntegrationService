using DiscordIntegrationService.Core.Interfaces;
using DiscordIntegrationService.Core.Models;
using DiscordIntegrationService.Core.Services;
using DiscordRPC;
using Moq;

namespace DiscordIntegrationService.UnitTests.Services;

[TestFixture]
public class DiscordPresenceUpdaterServiceTests
{
    [SetUp]
    public void Setup()
    {
        _settingsService = new Mock<ISettingsService>();
        _windowMonitorService = new Mock<IWindowMonitorService>();

        _settingsService.Setup(s => s.Load()).Returns(new Settings());
        _windowMonitorService.Setup(s => s.GetActiveWindow()).Returns(new WindowInfo
        {
            ProcessName = "VisualStudio",
            Title = "Coding"
        });
    }

    private Mock<ISettingsService> _settingsService;
    private Mock<IWindowMonitorService> _windowMonitorService;

    [Test]
    public async Task UpdatePresenceAsync_Should_Set_Presence()
    {
        var mockClient = new Mock<IDiscordRpcClient>();
        mockClient.Setup(c => c.IsInitialized).Returns(true);

        var updater = new DiscordPresenceUpdaterService(
            _settingsService.Object,
            _windowMonitorService.Object,
            mockClient.Object
        );

        var window = _windowMonitorService.Object.GetActiveWindow();

        await updater.UpdatePresenceAsync(window, CancellationToken.None);

        mockClient.Verify(c => c.SetPresence(It.Is<RichPresence>(rp =>
            rp.Details.Contains("VisualStudio") && rp.State.Contains("Coding")
        )), Times.Once);
    }
}