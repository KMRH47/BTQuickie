using System;
using System.Globalization;
using System.Windows.Data;

namespace BTQuickie.Converters;

public class PercentDecimalConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
    if (value is not double valueToMod) {
      return 0;
    }

    string? numberString = parameter.ToString()?.Replace(',', '.');

    bool isNumeric = double.TryParse(numberString, NumberStyles.Any, CultureInfo.InvariantCulture,
      out double percentDecimal);

    return isNumeric ? valueToMod * percentDecimal : 0;
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
    throw new NotImplementedException();
  }
}