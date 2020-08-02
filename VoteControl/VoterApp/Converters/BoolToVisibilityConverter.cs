using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VoterApp.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var typedValue = (bool)value;

            return typedValue ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var typed = (Visibility)value;

            switch (typed)
            {
                case Visibility.Collapsed:
                    return false;

                case Visibility.Hidden:
                    return false;

                case Visibility.Visible:
                    return true;

                default:
                    return false;
            }
        }
    }
}
