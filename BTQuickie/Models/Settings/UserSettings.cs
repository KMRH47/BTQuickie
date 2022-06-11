using System.Collections.ObjectModel;
using BTQuickie.Models.Hotkey;

namespace BTQuickie.Models.Settings;

public class UserSettings
{
    public ObservableCollection<HotkeyInfo> Keymap { get; set; } 
    public DiscoveryInfo DiscoveryInfo { get; set; }
}