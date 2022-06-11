using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;

namespace BTQuickie.Helpers;

public static class ScreenHelper
{
    /// <summary>
    /// Gets the screen size in pixels taking DPI scaling into consideration.
    /// </summary>
    /// <param name="window">The window to use as a reference for DPI information.</param>
    /// <param name="ignoreTaskbar">Whether or not to ignore the taskbar in the calculation.</param>
    /// <param name="useScreenFromCursorPos">If set to true, uses the screen where the cursor currently resides on.</param>
    /// <returns>The size of the screen in pixels after scaling.<b><br/>
    /// (double width, double height)</b>
    /// </returns>
    public static (double, double) GetScaledScreenSize(Window window, bool ignoreTaskbar = false, bool useScreenFromCursorPos = false)
    {
        IntPtr windowHandle = new WindowInteropHelper(window).Handle;
        Screen currentScreen = useScreenFromCursorPos ? Screen.FromPoint(Cursor.Position)  : Screen.FromHandle(windowHandle);
        Rectangle screenBounds = ignoreTaskbar ? currentScreen.WorkingArea : currentScreen.Bounds;
        DpiScale dpiScale = VisualTreeHelper.GetDpi(window);

        int screenX = screenBounds.Width + screenBounds.X;
        int screenY = screenBounds.Height + screenBounds.Y;
        int scaledWidth = (int) Math.Round(screenX / dpiScale.DpiScaleX);
        int scaledHeight = (int) Math.Round(screenY / dpiScale.DpiScaleY);

        return (scaledWidth, scaledHeight);
    }
}