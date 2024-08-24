using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using BTQuickie.Models.Device;
using BTQuickie.Services.Application;
using BTQuickie.Services.Bluetooth;
using BTQuickie.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BTQuickie.ViewModels;

public partial class MainViewModel(
  IBluetoothService bluetoothService,
  IApplicationContextProvider applicationContextProvider)
  : ViewModelBase
{
  [ObservableProperty] private IReadOnlyCollection<BluetoothDeviceInfo> devices = bluetoothService.GetPairedDevices();

  [ObservableProperty] private BluetoothDeviceInfo selectedBluetoothDeviceInfo;

  [RelayCommand]
  private async Task ConnectAsync(BluetoothDeviceInfo bluetoothDeviceInfo) {
    try {
      IsBusy = true;
      await bluetoothService.ConnectAsync(bluetoothDeviceInfo);
      Devices = [
        ..Devices.Where(device => device.Address != bluetoothDeviceInfo.Address),
        bluetoothDeviceInfo with { State = BluetoothConnectionState.Connected }
      ];
    }
    catch (SocketException e) {
      Debug.WriteLine($"{e.Message}\n{e.StackTrace}");
      Devices = [
        ..Devices.Where(device => device.Address != bluetoothDeviceInfo.Address),
        bluetoothDeviceInfo with { State = BluetoothConnectionState.Error }
      ];
    }
    finally {
      IsBusy = false;
    }
  }

  [RelayCommand(CanExecute = nameof(CanDiscoverBluetoothDevices))]
  private async Task DiscoverBluetoothDevicesAsync() {
    IsBusy = true;
    try {
      IReadOnlyCollection<BluetoothDeviceInfo> discoveredDevices = await Task.Run(bluetoothService.DiscoverDevices);
      Devices = [..discoveredDevices, ..bluetoothService.GetPairedDevices()];
    }
    finally {
      IsBusy = false;
    }
  }


  [RelayCommand]
  private void HideWindow() => applicationContextProvider.HideMainWindow();

  private bool CanDiscoverBluetoothDevices() => !IsBusy;
}