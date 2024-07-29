using System.Windows;

namespace BTQuickie.Services.Application;

public interface IApplicationContextProvider
{
  void Minimize();
  void Exit();
  void ShowMainWindow();
  void OpenWindow(string viewModelName);
  void HideMainWindow();

  /// <summary>
  ///   Retrieves the width and height of the screen where the cursor currently resides
  ///   scaled in relation to the parameters of this method.
  /// </summary>
  /// <example>
  ///   Screen width: 1920<br />
  ///   Screen height: 1080<br />
  ///   Parameters: (0.5, 0.5)<br />
  ///   Result: (960, 540)<br />
  ///   <code>
  /// (int scaledWidth, int scaledHeight) = GetDividedScreenSize("ExampleViewModel", 0.5, 0.5);
  /// scaledWidth = 960;
  /// scaledHeight = 540;
  /// </code>
  /// </example>
  Size GetScaledScreenSize(string viewModelName, float scaleWidthBy, float scaleHeightBy);
}