using DiscordIntegrationService.Core.Interfaces;
using DiscordIntegrationService.Core.Models;
using DiscordIntegrationService.Core.Utils;
using DiscordIntegrationService.UI.ViewModels;
using Moq;

namespace DiscordIntegrationService.UnitTests.ViewModels;

[TestFixture]
public class MainViewModelTests
{
    [Test]
    public void MainViewModel_Loads_And_Saves_ClientId()
    {
        var mockSettingsService = new Mock<ISettingsService>();
        var mockDiscordPresenceUpdaterService = new Mock<IDiscordPresenceUpdaterService>();

        mockSettingsService.Setup(s => s.Load()).Returns(new Settings());
        mockDiscordPresenceUpdaterService.Setup(s => s.TryReconnect(It.IsAny<Settings>()));

        var vm = new MainViewModel(mockSettingsService.Object, mockDiscordPresenceUpdaterService.Object)
        {
            DiscordClientId = "MyDiscordClientId"
        };

        vm.SaveCommand.Execute(null);

        mockSettingsService.Verify(s => s.Save(
                It.Is<Settings>(settings =>
                    !string.IsNullOrWhiteSpace(settings.DiscordClientId) &&
                    CryptoHelper.Deobfuscate(settings.DiscordClientId) == "MyDiscordClientId"
                )),
            Times.Once
        );
    }

    [Test]
    public void ExcludeList_Should_Be_Saved()
    {
        var mockSettingsService = new Mock<ISettingsService>();
        var mockDiscordPresenceUpdaterService = new Mock<IDiscordPresenceUpdaterService>();

        mockSettingsService.Setup(s => s.Load()).Returns(new Settings());
        mockDiscordPresenceUpdaterService.Setup(s => s.TryReconnect(It.IsAny<Settings>()));

        var vm = new MainViewModel(mockSettingsService.Object, mockDiscordPresenceUpdaterService.Object);
        vm.ExcludeList.Add("OBS");

        vm.SaveCommand.Execute(null);

        mockSettingsService.Verify(s => s.Save(
            It.Is<Settings>(settings => settings.ExcludeList.Contains("OBS"))
        ), Times.Once);
    }
}