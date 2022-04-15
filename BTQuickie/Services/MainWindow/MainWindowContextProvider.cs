using System;
using System.Windows;
using BTQuickie.Extensions;

namespace BTQuickie.Services.MainWindow;

public class MainWindowContextProvider : IMainWindowContextProvider
{
    public void Minimize()
    {
        MainWindow.WindowState = WindowState.Minimized;
    }

    public void Close()
    {
        MainWindow.Close();
    }

    public void Show()
    {     
        MainWindow.ShowBottomRightCorner();
    }

    public void Hide()
    {
        MainWindow.Hide();
    }

    private static Window MainWindow =>
        Application.Current.MainWindow is null
            ? throw new NullReferenceException($"{nameof(MainWindow)} is null.")
            : Application.Current.MainWindow;
}