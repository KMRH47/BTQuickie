using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using Cursor = System.Windows.Forms.Cursor;

namespace BTQuickie.Extensions;

internal static class WindowExtensions
{
    // Used for mutating the window's appearance
    private const int GWL_STYLE = -16;
    private const int WS_MAXIMIZEBOX = 0x10000;
    private const int WS_MINIMIZEBOX = 0x20000;

    // Used for registering hotkeys
    private const int HOTKEY_ID = 9000;
    private static HwndSource? _windowHandleSource;
    private static Window? _window;
    private static KeyBinding? _keyBinding;

    [DllImport("user32.dll")]
    private static extern int GetWindowLong(
        IntPtr windowHandle,
        int index);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(
        IntPtr windowHandle,
        int index,
        int value);

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

    /// <summary>
    /// This method and its associated code originates from a user on the MSDN forums.<br/>
    /// <a href="https://stackoverflow.com/a/339635/12186984"> Shared by StackOverflow user: Matt Hamilton</a><br/>
    /// <br/>
    /// Thanks to the unknown MSDN user and to Matt for sharing the code (time saved successfully).
    /// </summary>
    public static void HideMinimizeAndMaximizeButtons(this Window window)
    {
        IntPtr windowHandle = new WindowInteropHelper(window).Handle;
        int currentStyle = GetWindowLong(windowHandle, GWL_STYLE);

        SetWindowLong(windowHandle, GWL_STYLE, currentStyle & ~WS_MAXIMIZEBOX & ~WS_MINIMIZEBOX);
    }

    public static void RegisterHotkey(this Window window, KeyBinding keyBinding)
    {
        _keyBinding = keyBinding;
        _window = window;
        window.SourceInitialized += RegisterHotKey;
    }

    public static void UnregisterHotkey(this Window window)
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

    public static void ShowBottomRightCorner(this Window window)
    {
        Screen currentScreen = Screen.FromPoint(Cursor.Position);
        Rectangle screenBounds = currentScreen.WorkingArea;

        window.Left = screenBounds.Width + screenBounds.X - window.Width;
        window.Top = screenBounds.Height + screenBounds.Y - window.Height;
        window.ShowMinimal();
        window.Activate();
        window.Topmost = true;
    }

    public static void EnsureRendered(this Window window)
    {
        WindowInteropHelper helper = new(window);
        helper.EnsureHandle();
    }

    /// <summary>
    /// Shows window without a minimize and maximize button.
    /// </summary>
    private static void ShowMinimal(this Window window)
    {
        window.Show();
        HideMinimizeAndMaximizeButtons(window);
    }

    private static void RegisterHotKey(object? sender, EventArgs e)
    {
        if (_window is null)
        {
            return;
        }

        if (_keyBinding is null)
        {
            return;
        }

        WindowInteropHelper windowInteropHelper = new(_window);
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