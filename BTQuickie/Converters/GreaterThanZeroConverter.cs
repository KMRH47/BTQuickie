using System;
using System.Globalization;
using System.Windows.Data;

namespace BTQuickie.Converters;

public class GreaterThanZeroConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
    return value is > 0;
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
    throw new NotImplementedException();
  }
}