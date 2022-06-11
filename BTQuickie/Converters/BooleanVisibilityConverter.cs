using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BTQuickie.Converters;

public class BooleanVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not bool isVisible)
        {
            return Visibility.Visible;
        }

        return isVisible ? Visibility.Visible : Visibility.Hidden;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}