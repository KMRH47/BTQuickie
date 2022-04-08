using System.Windows;

namespace BTQuickie.Services.Application;

public class AppContextProvider : IAppContextProvider
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
        if (System.Windows.Application.Current.MainWindow is null)
        {
            return;
        }
        
        System.Windows.Application.Current.MainWindow.Show();
        System.Windows.Application.Current.MainWindow.Activate();
    }
}