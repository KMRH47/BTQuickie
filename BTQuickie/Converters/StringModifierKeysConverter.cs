using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;

namespace BTQuickie.Converters;

public class StringModifierKeysConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
    if (value is not string modifierKeys) {
      return ModifierKeys.None;
    }

    return new ModifierKeysConverter().ConvertFromString(modifierKeys) ?? ModifierKeys.None;
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
    if (value is not ModifierKeys modifierKeys) {
      return string.Empty;
    }

    return new ModifierKeysConverter().ConvertToString(modifierKeys) ?? string.Empty;
  }
}