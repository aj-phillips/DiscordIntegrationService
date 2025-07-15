using DiscordIntegrationService.Core.Models;
using DiscordIntegrationService.Core.Services;
using DiscordIntegrationService.Core.Utils;

namespace DiscordIntegrationService.UnitTests.Services;

[TestFixture]
public class SettingsServiceTests
{
    [Test]
    public void SaveAndLoad_Should_RoundTrip()
    {
        var filePath = Path.GetTempFileName();
        var service = new SettingsService(filePath);

        var original = new Settings
        {
            PollIntervalSeconds = 5,
            ExcludeList = ["Discord", "OBS"],
            DiscordClientId = CryptoHelper.Obfuscate("MyToken")
        };

        service.Save(original);
        var loaded = service.Load();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(loaded.PollIntervalSeconds, Is.EqualTo(original.PollIntervalSeconds));
            Assert.That(loaded.ExcludeList, Is.EqualTo(original.ExcludeList));
            Assert.That(loaded.DiscordClientId, Is.EqualTo(original.DiscordClientId));
        }

        File.Delete(filePath);
    }
}