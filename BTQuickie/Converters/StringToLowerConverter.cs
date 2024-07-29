using System;
using System.Globalization;
using System.Windows.Data;

namespace BTQuickie.Converters;

public class StringToLowerConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
    if (value is string text) {
      return text.ToLower();
    }

    throw new ArgumentException($"Argument '{nameof(value)}' must be a string.");
    ;
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
    throw new NotImplementedException();
  }
}