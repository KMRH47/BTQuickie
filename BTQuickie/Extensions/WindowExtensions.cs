using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace BTQuickie.Extensions;

/// <summary>
/// Code originally from a user on MSDN forums.<br/>
/// <a href="https://stackoverflow.com/a/339635/12186984"> Shared by StackOverflow user: Matt Hamilton</a><br/>
/// <br/>
/// Thanks to the unknown MSDN user and to Matt for sharing the code (time saved successfully).
/// </summary>
internal static class WindowExtensions
{
    private const int
        GWL_STYLE = -16,
        WS_MAXIMIZEBOX = 0x10000,
        WS_MINIMIZEBOX = 0x20000;

    [DllImport("user32.dll")]
    private static extern int GetWindowLong(IntPtr windowHandle, int index);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr windowHandle, int index, int value);

    internal static void HideMinimizeAndMaximizeButtons(this Window window)
    {
        IntPtr windowHandle = new System.Windows.Interop.WindowInteropHelper(window).Handle;
        int currentStyle = GetWindowLong(windowHandle, GWL_STYLE);

        SetWindowLong(windowHandle, GWL_STYLE, currentStyle & ~WS_MAXIMIZEBOX & ~WS_MINIMIZEBOX);
    }
}