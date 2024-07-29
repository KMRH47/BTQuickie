using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;

namespace BTQuickie.Converters;

public class StringKeyConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
    if (value is not string key) {
      return Key.None;
    }

    return new KeyConverter().ConvertFromString(key) ?? Key.None;
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
    if (value is not Key key) {
      return string.Empty;
    }

    return new KeyConverter().ConvertToString(key) ?? string.Empty;
  }
}