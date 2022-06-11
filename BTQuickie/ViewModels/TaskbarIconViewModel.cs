using System.Collections.Specialized;
using System.Linq;
using BTQuickie.Models.Hotkey;
using BTQuickie.Models.Settings;
using BTQuickie.Services.Application;
using BTQuickie.Services.Settings;
using BTQuickie.ViewModels.Base;
using CommunityToolkit.Mvvm.Input;

namespace BTQuickie.ViewModels;

public class TaskbarIconViewModel : ViewModelBase
{
    private readonly IApplicationContextProvider applicationContextProvider;
    private readonly UserSettings userSettings;

    public TaskbarIconViewModel(IApplicationContextProvider applicationContextProvider,
                                IApplicationSettingsProvider applicationSettingsProvider)
    {
        this.applicationContextProvider = applicationContextProvider;
        UserSettings userSettings = applicationSettingsProvider.UserSettings;
        userSettings.Keymap.CollectionChanged += OnKeymapChanged;
        this.userSettings = userSettings;;
    }

    public IRelayCommand OpenSettingsCommand => new RelayCommand(OpenSettings);
    public IRelayCommand ShowWindowCommand => new RelayCommand(ShowWindow);
    public IRelayCommand ExitCommand => new RelayCommand(Exit);

    public HotkeyInfo ShowBluetoothDevicesHotKey => this.userSettings.Keymap.First(hotkey => hotkey.Id is 0);

    private void OnKeymapChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action is not NotifyCollectionChangedAction.Add)
        {
            return;
        }

        OnPropertyChanged(nameof(ShowBluetoothDevicesHotKey));
    }

    private void ShowWindow()
    {
        this.applicationContextProvider.ShowMainWindow();
    }

    private void OpenSettings()
    {
        this.applicationContextProvider.OpenWindow(nameof(SettingsViewModel));
    }

    private void Exit()
    {
        this.applicationContextProvider.Exit();
    }
}