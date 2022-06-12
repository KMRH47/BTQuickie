using System;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using BTQuickie.Helpers;

namespace BTQuickie.Extensions;

internal static class WindowExtensions
{
    public static void ShowBottomRightCorner(this Window window)
    {
        (double width, double height) =
            ScreenHelper.GetScaledScreenSize(window, ignoreTaskbar: true, useScreenFromCursorPos: true);
        window.Left = width - window.Width;
        window.Top = height - window.Height;

        SetWindowStyle(window, 0);
        ShowTopmostAndActivate(window);
    }

    /// <summary>
    /// Shows window without the minimize and maximize button.
    /// </summary>
    public static void ShowMinimal(this Window window)
    {
        SetWindowStyle(window, ~WindowsInterop.WS.WS_MAXIMIZEBOX, ~WindowsInterop.WS.WS_MINIMIZEBOX);
        ShowTopmostAndActivate(window);
    }

    /// <summary>
    /// Ensures that a handle has been created for this window.
    /// </summary>
    /// <remarks>
    /// A <b>Handle to a WiNDow</b> (HWND) is automatically created when a window is shown/rendered within the WPF ecosystem.<br/><br/>
    /// Call this method if functionality is needed before the window is shown.
    /// </remarks>
    public static void EnsureHandle(this Window window)
    {
        WindowInteropHelper helper = new(window);
        helper.EnsureHandle();
    }

    /// <summary>
    /// Shows the window on top of all other windows and activates it.
    /// </summary>
    private static void ShowTopmostAndActivate(this Window window)
    {
        window.Show();
        window.Topmost = true;
        window.Activate();
    }

    private static void SetWindowStyle(this Window window, params int[] styles)
    {
        IntPtr windowHandle = new WindowInteropHelper(window).Handle;
        int currentStyle = WindowsInterop.Windows.GetWindowLongA((WindowsInterop.HWND)windowHandle, WindowsInterop.GWL.GWL_STYLE);
        int newStyle = styles.Aggregate(currentStyle, (current, style) => current & style);
        WindowsInterop.Windows.SetWindowLongA((WindowsInterop.HWND)windowHandle, WindowsInterop.GWL.GWL_STYLE, newStyle);
    }
}