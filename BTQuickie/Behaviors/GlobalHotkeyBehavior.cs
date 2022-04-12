using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using BTQuickie.Extensions;

namespace BTQuickie.Behaviors;

public class GlobalHotkeyBehavior
{
    [AttachedPropertyBrowsableForType(typeof(KeyBinding))]
    public static bool GetRegister(KeyBinding binding)
    {
        return (bool) binding.GetValue(RegisterProperty);
    }

    public static void SetRegister(KeyBinding binding, bool value)
    {
        binding.SetValue(RegisterProperty, value);
    }

    public static readonly DependencyProperty RegisterProperty =
        DependencyProperty.RegisterAttached(
            "Register",
            typeof(bool),
            typeof(GlobalHotkeyBehavior),
            new PropertyMetadata(
                false,
                RegisterPropertyChanged));

    private static void RegisterPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
        if (dependencyObject is not KeyBinding keyBinding)
        {
            return;
        }

        bool isDesignMode = DesignerProperties.GetIsInDesignMode(dependencyObject);

        if (isDesignMode)
        {
            return;
        }

        bool oldValue = (bool) e.OldValue;
        bool newValue = (bool) e.NewValue;

        Window window = Application.Current.MainWindow!;

        if (oldValue && !newValue)
        {
            window.UnregisterHotkey();
        }
        else if (newValue && !oldValue)
        {
            window.RegisterHotkey(keyBinding);
        }
    }
}