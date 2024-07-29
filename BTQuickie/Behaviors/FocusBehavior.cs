using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace BTQuickie.Behaviors;

public class FocusBehavior
{
  public static readonly DependencyProperty IsFocusedProperty =
    DependencyProperty.RegisterAttached(
      "IsFocused",
      typeof(bool),
      typeof(FocusBehavior),
      new PropertyMetadata(false, OnFocusFirstPropertyChanged));

  public static bool GetIsFocused(Control control) {
    return (bool)control.GetValue(IsFocusedProperty);
  }

  /// <summary>
  ///   Forces focus on this element when its parent window is shown.
  /// </summary>
  public static void SetIsFocused(Control control, bool value) {
    control.SetValue(IsFocusedProperty, value);
  }

  private static void OnFocusFirstPropertyChanged(DependencyObject dependencyObject,
    DependencyPropertyChangedEventArgs args) {
    if (dependencyObject is not Control control) {
      return;
    }

    if (args.NewValue is not true) {
      return;
    }

    control.Loaded += OnControlLoaded;
  }

  private static void OnControlLoaded(object sender, RoutedEventArgs e) {
    if (sender is not Control control) {
      return;
    }

    if (control is not Selector selector) {
      control.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
      return;
    }

    if (selector.Items.Count <= 0) {
      control.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
      return;
    }

    if (selector.ItemContainerGenerator.ContainerFromIndex(0)
        is not ListBoxItem listBoxItem) {
      control.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
      return;
    }

    selector.SelectedIndex = 0;
    listBoxItem.Focus();
  }
}