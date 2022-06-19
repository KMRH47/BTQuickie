using System.Collections.ObjectModel;
using BTQuickie.Helpers;
using BTQuickie.Models.Hotkey;

namespace BTQuickie.Models.Settings;

public class UserSettings
{
    private bool launchOnStartup;
    public ObservableCollection<HotkeyInfo> Keymap { get; init; }
    public DiscoveryInfo DiscoveryInfo { get; init; }

    public bool LaunchOnStartup
    {
        get => this.launchOnStartup;
        set
        {
            this.launchOnStartup = value;

            if (value)
            {
                ShellLinkHelper.CreateStartupShortcut();
                return;
            }

            ShellLinkHelper.RemoveStartupShortcut();
        }
    }
}