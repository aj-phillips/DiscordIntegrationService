using System.Text;

namespace DiscordIntegrationService.Core.Utils;

public static class CryptoHelper
{
    private const string Salt = "Iqb@kIVBK2k*ad";

    public static string Obfuscate(string plainText)
    {
        var combined = Salt + plainText;
        var bytes = Encoding.UTF8.GetBytes(combined);

        return Convert.ToBase64String(bytes);
    }

    public static string Deobfuscate(string obfuscatedText)
    {
        if (string.IsNullOrWhiteSpace(obfuscatedText))
            return string.Empty;

        var bytes = Convert.FromBase64String(obfuscatedText);
        var combined = Encoding.UTF8.GetString(bytes);

        return combined.StartsWith(Salt)
            ? combined[Salt.Length..]
            : throw new InvalidOperationException("Invalid format");
    }
}