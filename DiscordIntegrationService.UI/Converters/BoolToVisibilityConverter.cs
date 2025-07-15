using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DiscordIntegrationService.UI.Converters;

public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var visible = (bool)value;
        var invert = parameter != null && bool.Parse(parameter.ToString());
        if (invert) visible = !visible;

        return visible ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}