using DiscordIntegrationService.Core.Interfaces;
using DiscordIntegrationService.Core.Models;
using DiscordIntegrationService.UI.Services;
using Moq;

namespace DiscordIntegrationService.UnitTests.Worker;

[TestFixture]
public class WorkerServiceTests
{
    [Test]
    public void Worker_Should_Not_Update_When_Window_Excluded()
    {
        var window = new WindowInfo { ProcessName = "Discord", Title = "Game" };
        var settings = new Settings { ExcludeList = ["Discord"] };

        var worker = new WorkerService(
            new Mock<IWindowMonitorService>().Object,
            new Mock<IDiscordPresenceUpdaterService>().Object,
            new Mock<ISettingsService>().Object,
            new Mock<IPresenceUpdateDecisionService>().Object
        );

        var shouldUpdate = worker.ShouldUpdatePresence(window, null, settings);

        Assert.That(shouldUpdate, Is.False);
    }
}