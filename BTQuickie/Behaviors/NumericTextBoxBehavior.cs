using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BTQuickie.Behaviors;

public class NumericTextBoxBehavior
{
  public static readonly DependencyProperty IsNumericInputProperty =
    DependencyProperty.RegisterAttached(
      "NumericInput",
      typeof(bool),
      typeof(NumericTextBoxBehavior),
      new PropertyMetadata(false, IsNumericInputPropertyChanged));

  public static bool GetIsNumericInput(Control control) {
    return (bool)control.GetValue(IsNumericInputProperty);
  }

  /// <summary>
  ///   Forces focus on this element when its parent window is shown.
  /// </summary>
  public static void SetIsNumericInput(Control control, bool value) {
    control.SetValue(IsNumericInputProperty, value);
  }

  private static void IsNumericInputPropertyChanged(DependencyObject dependencyObject,
    DependencyPropertyChangedEventArgs args) {
    if (dependencyObject is not TextBox textBox) {
      return;
    }

    if (args.NewValue is not true) {
      return;
    }

    textBox.PreviewTextInput += TextBoxOnPreviewTextInput;
  }

  private static void TextBoxOnPreviewTextInput(object sender, TextCompositionEventArgs e) {
    string text = e.Text;

    if (text.Length <= 0) {
      e.Handled = true;
      return;
    }

    bool isNotDigit = !char.IsDigit(text[^1]);
    e.Handled = isNotDigit;
  }
}