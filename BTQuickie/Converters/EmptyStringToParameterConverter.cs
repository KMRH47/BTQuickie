using System;
using System.Globalization;
using System.Windows.Data;

namespace BTQuickie.Converters;

public class EmptyStringToParameterConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
    if (value is string text && string.IsNullOrEmpty(text)) {
      return parameter;
    }

    return value;
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
    throw new NotImplementedException();
  }
}