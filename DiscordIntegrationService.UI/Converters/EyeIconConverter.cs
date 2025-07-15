using System.Globalization;
using System.Windows.Data;

namespace DiscordIntegrationService.UI.Converters;

public class EyeIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? "🙈" : "👁️";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}