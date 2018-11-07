using System;
using System.Globalization;
using System.Windows.Data;

namespace Core.Converter
{
    public class BindingEqualsParamConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var v = value?.ToString();
            var p = parameter?.ToString();

            return string.Equals(v, p);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool v && v == true)
            {
                return parameter;
            }

            return Binding.DoNothing;
        }
    }
}
