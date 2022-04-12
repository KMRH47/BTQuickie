using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BTQuickie.Behaviors;

public class ForceFocusBehavior
{
    public static readonly DependencyProperty FocusProperty =
        DependencyProperty.RegisterAttached(
            "Focus",
            typeof(bool),
            typeof(ForceFocusBehavior),
            new PropertyMetadata(false, OnFocusFirstPropertyChanged));

    public static bool GetFocus(Control control)
    {
        return (bool) control.GetValue(FocusProperty);
    }

    public static void SetFocus(Control control, bool value)
    {
        control.SetValue(FocusProperty, value);
    }

    private static void OnFocusFirstPropertyChanged(DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is not Control control)
        {
            return;
        }

        if (args.NewValue is not true)
        {
            return;
        }

        control.Loaded += (_, _) => control.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
    }
}