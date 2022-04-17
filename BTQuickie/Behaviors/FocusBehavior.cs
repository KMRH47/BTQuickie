using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BTQuickie.Behaviors;

public class FocusBehavior
{
   
    public static readonly DependencyProperty IsForcedProperty =
        DependencyProperty.RegisterAttached(
            "IsForced",
            typeof(bool),
            typeof(FocusBehavior),
            new PropertyMetadata(false, OnFocusFirstPropertyChanged));

    public static bool GetIsForced(Control control)
    {
        return (bool) control.GetValue(IsForcedProperty);
    }

    /// <summary>
    /// Forces focus on this element when its parent window is shown.
    /// </summary>
    public static void SetIsForced(Control control, bool value)
    {
        control.SetValue(IsForcedProperty, value);
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