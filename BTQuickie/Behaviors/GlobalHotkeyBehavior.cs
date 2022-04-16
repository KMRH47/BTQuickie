using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace BTQuickie.Behaviors;

public class GlobalHotkeyBehavior
{
    private const int HOTKEY_ID = 9000;
    private static HwndSource? _windowHandleSource;
    private static KeyBinding? _keyBinding;
    
    [DllImport("User32.dll")]
    private static extern bool RegisterHotKey(
        IntPtr windowHandle,
        int hotkeyId,
        uint fsModifiers,
        uint virtualKey);

    [DllImport("User32.dll")]
    private static extern bool UnregisterHotKey(
        IntPtr windowHandle,
        int hotkeyId);
    
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

        bool oldValue = (bool) e.OldValue;
        bool newValue = (bool) e.NewValue;

        Window window = Application.Current.MainWindow!;

        if (oldValue && !newValue)
        {
           UnregisterHotkey(window);
        }
        else if (newValue && !oldValue)
        {
            _keyBinding = keyBinding;
            window.SourceInitialized += RegisterHotKey;
        }
    }

    private static void RegisterHotKey(object? sender, EventArgs e)
    {
        if (sender is not Window window)
        {
            return;
        }

        if (_keyBinding is null)
        {
            return;
        }

        WindowInteropHelper windowInteropHelper = new(window);
        uint key = (uint) KeyInterop.VirtualKeyFromKey(_keyBinding.Key);
        uint modifiers = (uint) _keyBinding.Modifiers;

        _windowHandleSource = HwndSource.FromHwnd(windowInteropHelper.Handle) ?? throw new InvalidOperationException();
        _windowHandleSource.AddHook(WindowHandleHook);

        bool hotkeyRegisterError = !RegisterHotKey(windowInteropHelper.Handle, HOTKEY_ID, modifiers, key);

        if (hotkeyRegisterError)
        {
            Debug.WriteLine($"Hotkey couldn't be registered...");
        }
    }
    
    private static void UnregisterHotkey(Window window)
    {
        if (_windowHandleSource is null)
        {
            return;
        }

        _keyBinding = null;
        _windowHandleSource.RemoveHook(WindowHandleHook);
        WindowInteropHelper windowInteropHelper = new(window);
        UnregisterHotKey(windowInteropHelper.Handle, HOTKEY_ID);
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

        _keyBinding?.Command?.Execute(_keyBinding.CommandParameter);
        isHandled = true;

        return IntPtr.Zero;
    }
}