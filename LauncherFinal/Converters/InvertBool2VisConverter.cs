using System;
using System.Globalization;
using System.Windows.Data;
using MaterialDesignThemes.Wpf.Converters;

namespace LauncherFinal.Converters
{
    class InvertBool2VisConverter : IValueConverter
    {
        private readonly IValueConverter _bool2VisConverter = new BooleanToVisibilityConverter();


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool param)
            {
                return _bool2VisConverter.Convert(!param, targetType, parameter, culture);
            }

            return _bool2VisConverter.Convert(value, targetType, parameter, culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = _bool2VisConverter.ConvertBack(value, targetType, parameter, culture) as bool?;

            return result != true;
        }
    }
}
