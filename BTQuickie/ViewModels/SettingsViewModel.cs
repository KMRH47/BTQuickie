using System.Collections.ObjectModel;
using System.ComponentModel;
using BTQuickie.Models.Hotkey;
using BTQuickie.Services.Settings;
using BTQuickie.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BTQuickie.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    private readonly IApplicationSettingsProvider applicationSettingsProvider;
    
    [ObservableProperty]
    private HotkeyInfo selectedHotkey = HotkeyInfo.Empty;

    public SettingsViewModel(IApplicationSettingsProvider applicationSettingsProvider)
    {
        this.applicationSettingsProvider = applicationSettingsProvider;
    }

    public ObservableCollection<HotkeyInfo> Keymap => this.applicationSettingsProvider.UserSettings.Keymap;
    
    public bool LaunchOnStartup
    {
        get => this.applicationSettingsProvider.UserSettings.LaunchOnStartup;
        set => this.applicationSettingsProvider.UserSettings.LaunchOnStartup = value;
    }

    public int DiscoveryTimeMs
    {
        get => this.applicationSettingsProvider.UserSettings.DiscoveryInfo.DiscoveryTimeMs;
        set
        {
            this.applicationSettingsProvider.UserSettings.DiscoveryInfo.DiscoveryTimeMs = value;
            OnPropertyChanged();
        }
    }

    public Hotkey? BoundHotkey
    {
        set
        {
            if (value is null)
            {
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

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        this.applicationSettingsProvider.WriteUserSettings();
    }
}