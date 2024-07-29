using System.Windows.Input;

namespace BTQuickie.Extensions;

public static class KeyEventArgsExtensions
{
  public static Key RealKey(this KeyEventArgs e) {
    return e.Key switch {
      Key.System => e.SystemKey,
      Key.ImeProcessed => e.ImeProcessedKey,
      Key.DeadCharProcessed => e.DeadCharProcessedKey,
      _ => e.Key
    };
  }

  /// <summary>
  ///   Determines if the <see cref="KeyEventArgs.Key">key</see> is a modifier key.
  /// </summary>
  /// <returns>
  ///   True if the key is a <see cref="System.Windows.Input.ModifierKeys">modifier key</see>
  ///   , otherwise false.
  /// </returns>
  public static bool IsModifierKey(this KeyEventArgs e) {
    switch (e.RealKey()) {
      case Key.LeftCtrl:
      case Key.RightCtrl:
      case Key.LeftAlt:
      case Key.RightAlt:
      case Key.LeftShift:
      case Key.RightShift:
      case Key.LWin:
      case Key.RWin:
        return true;
      default:
        return false;
    }
  }

  /// <summary>
  ///   Used to determine which modifier are currently pressed.
  /// </summary>
  /// <returns>
  ///   Returns the <see cref="System.Windows.Input.ModifierKeys">modifier keys</see> currently pressed
  ///   (including <see cref="System.Windows.Input.ModifierKeys.Windows" />).
  /// </returns>
  public static ModifierKeys ModifierKeys(this KeyEventArgs e) {
    switch (e.RealKey()) {
      case Key.LeftCtrl:
      case Key.RightCtrl:
      case Key.LeftAlt:
      case Key.RightAlt:
      case Key.LeftShift:
      case Key.RightShift:
        bool isWinDown = e.KeyboardDevice.IsKeyDown(Key.LWin) || e.KeyboardDevice.IsKeyDown(Key.RWin);
        return isWinDown ? Keyboard.Modifiers | System.Windows.Input.ModifierKeys.Windows : Keyboard.Modifiers;
      case Key.LWin:
      case Key.RWin:
        if (Keyboard.Modifiers == System.Windows.Input.ModifierKeys.None) {
          return System.Windows.Input.ModifierKeys.Windows;
        }

        return Keyboard.Modifiers | System.Windows.Input.ModifierKeys.Windows;
      default:
        return System.Windows.Input.ModifierKeys.None;
    }
  }

  /// <summary>
  ///   Determines if multiple identical modifier keys are held down simultaneously.
  /// </summary>
  /// <param name="e">KeyEventArgs</param>
  /// <returns>True if e.g. left control and right control are held down simultaneously.</returns>
  public static bool IdenticalModifierKeyStroke(this KeyEventArgs e) {
    switch (e.Key) {
      case Key.LeftCtrl:
      case Key.RightCtrl:
        return Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.RightCtrl);
      case Key.LeftAlt:
      case Key.RightAlt:
        return Keyboard.IsKeyDown(Key.LeftAlt) && Keyboard.IsKeyDown(Key.RightAlt);
      case Key.LeftShift:
      case Key.RightShift:
        return Keyboard.IsKeyDown(Key.LeftShift) && Keyboard.IsKeyDown(Key.RightShift);
      case Key.LWin:
      case Key.RWin:
        return Keyboard.IsKeyDown(Key.LWin) && Keyboard.IsKeyDown(Key.RWin);
      default:
        return false;
    }
  }
}