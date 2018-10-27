using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace LauncherFinal.Converters
{
    class MathConverter : IValueConverter
    {
        private double _divide = 1;

        public double Divide
        {
            get => _divide;
            set => _divide = value;
        }

        public double Add { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (double.TryParse(value?.ToString(), out var val))
            {
                return val / Divide + Add;
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
