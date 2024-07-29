using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ShellLink;
using ShellLink.Structures;

namespace BTQuickie.Helpers;

public static class ShellLinkHelper
{
  private const string ShellLinkExtension = "lnk";
  private static readonly string ApplicationName = Process.GetCurrentProcess().ProcessName;
  private static readonly string StartupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup);

  private static readonly string ApplicationDir = Environment.GetCommandLineArgs()[0]
    .Replace($"\\{ApplicationName}.dll", "");

  private static readonly string ExePath = $"{ApplicationDir}\\{ApplicationName}.exe";

  /// <summary>
  ///   Creates a shortcut (.lnk) file in the startup folder with a name identical to the process name.<br />
  /// </summary>
  /// <remarks>
  ///   If the shortcut already exists, no action will be taken.
  /// </remarks>
  public static void CreateStartupShortcut() {
    if (StartupShortcutExists()) {
      return;
    }

    string shortcutPath = Path.Combine(StartupPath, $"{ApplicationName}.{ShellLinkExtension}");
    var shortcut = Shortcut.CreateShortcut(ExePath);
    StringData stringData = new() { WorkingDir = ApplicationDir };
    shortcut.StringData = stringData;
    shortcut.WriteToFile(shortcutPath);
  }

  /// <summary>
  ///   Attempts to remove  startup shortcut with a name identical to the process name.<br />
  /// </summary>
  /// <remarks>
  ///   If the shortcut does not exist, no action will be taken.
  /// </remarks>
  public static void RemoveStartupShortcut() {
    if (!StartupShortcutExists()) {
      return;
    }

    string[] files = Directory.GetFiles(StartupPath, $"{ApplicationName}*.*");
    string shortcutPath = files.First(s => s.Contains(ShellLinkExtension));
    File.Delete(shortcutPath);
  }

  /// <summary>
  ///   Determines if a shortcut (.lnk) with a name identical to the process name exists in the startup folder.
  /// </summary>
  /// <returns></returns>
  private static bool StartupShortcutExists() {
    string[] files = Directory.GetFiles(StartupPath, $"{ApplicationName}*.*");
    return files.Any(s => s.Contains(ShellLinkExtension));
  }
}