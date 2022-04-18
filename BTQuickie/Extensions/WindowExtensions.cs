using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using Cursor = System.Windows.Forms.Cursor;

namespace BTQuickie.Extensions;

internal static class WindowExtensions
{
    // Used for mutating the window's appearance
    private const int GWL_STYLE = -16;
    private const int WS_MAXIMIZEBOX = 0x10000;
    private const int WS_MINIMIZEBOX = 0x20000;
   
    [DllImport("user32.dll")]
    private static extern int GetWindowLong(
        IntPtr windowHandle,
        int index);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(
        IntPtr windowHandle,
        int index,
        int value);

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
    /// Shows window without the minimize and maximize button.
    /// </summary>
    private static void ShowMinimal(this Window window)
    {
        EnsureRendered(window);
        HideMinimizeAndMaximizeButtons(window);
        window.Show();
    }
    
    /// <summary>
    /// This method and its associated code originates from a user on the MSDN forums.<br/>
    /// <a href="https://stackoverflow.com/a/339635/12186984"> Shared by StackOverflow user: Matt Hamilton</a><br/>
    /// <br/>
    /// Thanks to the unknown MSDN user and to Matt for sharing the code (time saved successfully).
    /// </summary>
    private static void HideMinimizeAndMaximizeButtons(this Window window)
    {
        IntPtr windowHandle = new WindowInteropHelper(window).Handle;
        int currentStyle = GetWindowLong(windowHandle, GWL_STYLE);

        SetWindowLong(windowHandle, GWL_STYLE, currentStyle & ~WS_MAXIMIZEBOX & ~WS_MINIMIZEBOX);
    }
}