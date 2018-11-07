using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Configurator.Converters
{
    public class VersionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Version version)
            {
                return version.ToString();
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Version.TryParse(value?.ToString(), out var version))
            {
                return version;
            }

            return DependencyProperty.UnsetValue;
        }
    }
}
