using System;
using System.Globalization;
using System.Windows.Data;

namespace VoterApp.Converters
{
    public class VoteOptionToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var typed = (VoteOption)value;
            var typedParameter = (VoteOption)parameter;

            return typed == typedParameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var typedParameter = (VoteOption)parameter;

            return typedParameter;
        }
    }
}
