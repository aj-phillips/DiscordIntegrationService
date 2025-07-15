using DiscordIntegrationService.Core.Services;

namespace DiscordIntegrationService.UnitTests.Services;

[TestFixture]
public class WindowMonitorServiceTests
{
    [Test]
    public void GetActiveWindow_Should_Return_NonNull()
    {
        var monitor = new WindowMonitorService();
        var info = monitor.GetActiveWindow();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(info, Is.Not.Null);
            Assert.That(string.IsNullOrWhiteSpace(info.Title), Is.False);
            Assert.That(string.IsNullOrWhiteSpace(info.ProcessName), Is.False);
        }
    }
}