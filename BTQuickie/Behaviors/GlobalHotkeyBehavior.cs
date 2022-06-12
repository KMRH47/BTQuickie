using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace BTQuickie.Behaviors;

public class GlobalHotkeyBehavior
{
    private const int HOTKEY_ID = 9000;
    private static HwndSource? _windowHandleSource;
    private static KeyBinding _keyBinding = new();
    private static Key _key;
    private static ModifierKeys _modifierKeys;

    public static readonly DependencyProperty RegisterProperty =
        DependencyProperty.RegisterAttached(
            "Register",
            typeof(bool),
            typeof(GlobalHotkeyBehavior),
            new PropertyMetadata(
                false,
                RegisterPropertyChanged));

    [AttachedPropertyBrowsableForType(typeof(KeyBinding))]
    public static bool GetRegister(KeyBinding binding)
    {
        return (bool) binding.GetValue(RegisterProperty);
    }

    public static void SetRegister(KeyBinding binding, bool value)
    {
        binding.SetValue(RegisterProperty, value);
    }
    
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

        _keyBinding = keyBinding;
        keyBinding.Changed += KeyBindingOnChanged;
        KeyBindingOnChanged(keyBinding, EventArgs.Empty);
    }

    private static void KeyBindingOnChanged(object? sender, EventArgs e)
    {
        
        if (_key == _keyBinding.Key && _modifierKeys == _keyBinding.Modifiers)
        {
            return;
        }

        _key = _keyBinding.Key;
        _modifierKeys = _keyBinding.Modifiers;

        Window window = Application.Current.MainWindow!;
        OnUnregisterHotkey(window);
        OnRegisterHotKey(window);
    }

    private static void OnRegisterHotKey(Window window)
    {
        WindowInteropHelper windowInteropHelper = new(window);
        uint key = (uint) KeyInterop.VirtualKeyFromKey(_keyBinding.Key);
        uint modifiers = (uint) _keyBinding.Modifiers;

        _windowHandleSource = HwndSource.FromHwnd(windowInteropHelper.Handle) ?? throw new InvalidOperationException();
        _windowHandleSource.AddHook(WindowHandleHook);

        bool hotkeyRegisterError = !WindowsInterop.Windows.RegisterHotKey((WindowsInterop.HWND)windowInteropHelper.Handle, HOTKEY_ID, modifiers, key);

        if (hotkeyRegisterError)
        {
            Debug.WriteLine($"Hotkey couldn't be registered...");
        }
    }

    private static void OnUnregisterHotkey(Window window)
    {
        if (_windowHandleSource is null)
        {
            return;
        }

        _windowHandleSource.RemoveHook(WindowHandleHook);
        WindowInteropHelper windowInteropHelper = new(window);
        WindowsInterop.Windows.UnregisterHotKey((WindowsInterop.HWND)windowInteropHelper.Handle, HOTKEY_ID);
    }

    private static IntPtr WindowHandleHook(
        IntPtr windowHandle,
        int message,
        IntPtr wParam,
        IntPtr lParam,
        ref bool isHandled)
    {
        const int WM_HOTKEY = 0x0312;

        if (message != WM_HOTKEY)
        {
            return IntPtr.Zero;
        }

        if (wParam.ToInt32() != HOTKEY_ID)
        {
            return IntPtr.Zero;
        }

        if (Keyboard.FocusedElement is not null)
        {
            return IntPtr.Zero;
        }

        _keyBinding.Command.Execute(_keyBinding.CommandParameter);

        isHandled = true;

        return IntPtr.Zero;
    }
}