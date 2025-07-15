using DiscordIntegrationService.Core.Models;
using DiscordIntegrationService.Core.Services;

namespace DiscordIntegrationService.UnitTests.Services;

[TestFixture]
public class PresenceUpdateDecisionServiceTests
{
    [Test]
    public void ShouldUpdate_Returns_True_When_NewWindow_NotExcluded()
    {
        var decision = new PresenceUpdateDecisionService();
        var settings = new Settings { ExcludeList = ["Discord"] };

        var last = new WindowInfo { ProcessName = "VS", Title = "Old" };
        var next = new WindowInfo { ProcessName = "Notepad", Title = "NewFile.txt" };

        var result = decision.ShouldUpdate(next, last, settings);

        Assert.That(result, Is.True);
    }

    [Test]
    public void ShouldUpdate_Returns_False_When_SameWindow()
    {
        var decision = new PresenceUpdateDecisionService();
        var settings = new Settings();

        var last = new WindowInfo { ProcessName = "Notepad", Title = "File.txt" };
        var next = new WindowInfo { ProcessName = "Notepad", Title = "File.txt" };

        Assert.That(decision.ShouldUpdate(next, last, settings), Is.False);
    }

    [Test]
    public void ShouldUpdate_Returns_False_When_Excluded()
    {
        var decision = new PresenceUpdateDecisionService();
        var settings = new Settings { ExcludeList = ["Notepad"] };

        var next = new WindowInfo { ProcessName = "Notepad", Title = "File.txt" };

        Assert.That(decision.ShouldUpdate(next, null, settings), Is.False);
    }
}