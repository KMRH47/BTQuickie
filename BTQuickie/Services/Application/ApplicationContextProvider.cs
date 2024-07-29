using System;
using System.Windows;
using BTQuickie.Extensions;
using BTQuickie.Helpers;

namespace BTQuickie.Services.Application;

public class ApplicationContextProvider : IApplicationContextProvider
{
  private static Window MainWindow =>
    System.Windows.Application.Current.MainWindow is not null
      ? System.Windows.Application.Current.MainWindow
      : throw new NullReferenceException($"{nameof(MainWindow)} is null.");

  public void Exit() => System.Windows.Application.Current.Shutdown();

  public void Minimize() => MainWindow.WindowState = WindowState.Minimized;

  public void HideMainWindow() => MainWindow.Hide();

  public void OpenWindow(string viewModelName) => GetWindowByName(viewModelName).ShowMinimal();

  public void ShowMainWindow() {
    Window mainWindow = MainWindow;

    if (mainWindow.IsVisible) {
      HideMainWindow();
    }

    mainWindow.ShowBottomRightCorner();
  }

  public Size GetScaledScreenSize(string viewModelName, float scaleWidthBy, float scaleHeightBy) {
    Window window = GetWindowByName(viewModelName);
    (double scaledWidth, double scaledHeight) = ScreenHelper.GetScaledScreenSize(window, false, true);
    return new Size(scaleWidthBy * scaledWidth, scaleHeightBy * scaledHeight);
  }

  private static Window GetWindowByName(string viewModelName) {
    string windowName = viewModelName.Replace("Model", string.Empty);

    foreach (Window window in System.Windows.Application.Current.Windows) {
      if (window.GetType().Name != windowName) {
        continue;
      }

      return window;
    }

    throw new MissingMemberException($"{nameof(Window)} '{windowName}' does not exist.");
  }
}