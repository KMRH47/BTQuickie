using System.Collections.ObjectModel;
using System.ComponentModel;
using BTQuickie.Models.Hotkey;
using BTQuickie.Services.Settings;
using BTQuickie.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BTQuickie.ViewModels;

public partial class SettingsViewModel(IApplicationSettingsProvider applicationSettingsProvider) : ViewModelBase
{
  [ObservableProperty] private HotkeyInfo selectedHotkey = HotkeyInfo.Empty;

  public ObservableCollection<HotkeyInfo> Keymap => applicationSettingsProvider.UserSettings.Keymap;

  public bool LaunchOnStartup {
    get => applicationSettingsProvider.UserSettings.LaunchOnStartup;
    set => applicationSettingsProvider.UserSettings.LaunchOnStartup = value;
  }

  public int DiscoveryTimeMs {
    get => applicationSettingsProvider.UserSettings.DiscoveryInfo.DiscoveryTimeMs;
    set {
      applicationSettingsProvider.UserSettings.DiscoveryInfo.DiscoveryTimeMs = value;
      OnPropertyChanged();
    }
  }

  public Hotkey? BoundHotkey {
    set {
      if (value is null) {
        return;
      }

      HotkeyInfo hotkey =
        new(value.Key,
          value.Modifiers,
          SelectedHotkey.Id,
          SelectedHotkey.Description);

      Keymap.Remove(SelectedHotkey);
      Keymap.Add(hotkey);
      SelectedHotkey = hotkey;
      OnPropertyChanged();
    }
  }

  protected override void OnPropertyChanged(PropertyChangedEventArgs e) {
    base.OnPropertyChanged(e);
    applicationSettingsProvider.WriteUserSettings();
  }
}