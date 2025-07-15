using DiscordIntegrationService.Core.Utils;

namespace DiscordIntegrationService.UnitTests.Utilities;

[TestFixture]
public class CryptoHelperTests
{
    [Test]
    public void Should_Obfuscate_And_Deobfuscate_Correctly()
    {
        const string plain = "MySecretClientId123";

        var obfuscated = CryptoHelper.Obfuscate(plain);
        Assert.That(obfuscated, Is.Not.Null);

        var deobfuscated = CryptoHelper.Deobfuscate(obfuscated);
        Assert.That(deobfuscated, Is.EqualTo(plain));
    }

    [Test]
    public void Should_Fail_When_Deobfuscating_Tampered()
    {
        const string plain = "MySecret";
        var obfuscated = CryptoHelper.Obfuscate(plain);

        var tampered = obfuscated.Replace("M", "X"); // break it

        Assert.Throws<InvalidOperationException>(() => CryptoHelper.Deobfuscate(tampered));
    }
}