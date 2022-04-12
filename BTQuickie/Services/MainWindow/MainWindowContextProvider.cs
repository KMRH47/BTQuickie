using System.Windows;
using System.Windows.Forms;
using BTQuickie.Extensions;
using Point = System.Drawing.Point;

namespace BTQuickie.Services.MainWindow;

public class MainWindowContextProvider : IMainWindowContextProvider
{
    public void Minimize()
    {
        if (System.Windows.Application.Current.MainWindow is null)
        {
            return;
        }

        System.Windows.Application.Current.MainWindow.WindowState = WindowState.Minimized;
    }

    public void Close()
    {
        if (System.Windows.Application.Current.MainWindow is null)
        {
            return;
        }

        System.Windows.Application.Current.MainWindow.Close();
    }

    public void Show()
    {
        Window? mainWindow = System.Windows.Application.Current.MainWindow;

        if (mainWindow is null)
        {
            return;
        }

        mainWindow.ShowMinimal();
        mainWindow.Activate();
        mainWindow.Topmost = true;

        Point mousePos = Control.MousePosition;
        mainWindow.Left = mousePos.X - (mainWindow.Width/2);
        mainWindow.Top = mousePos.Y - mainWindow.Height;
    }
}