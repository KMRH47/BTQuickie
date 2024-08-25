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
  [ObservableProperty] private IReadOnlyCollection<BluetoothDevice> devices = bluetoothService.GetPairedDevices();

  [ObservableProperty] private BluetoothDevice selectedBluetoothDevice;

  [RelayCommand]
  private async Task ConnectAsync(BluetoothDevice bluetoothDevice) {
    try {
      IsBusy = true;
      await bluetoothService.ConnectAsync(bluetoothDevice);
      Devices = [
        ..Devices.Where(device => device.Address != bluetoothDevice.Address),
        bluetoothDevice with { State = BluetoothConnectionState.Connected }
      ];
    }
    catch (SocketException e) {
      Debug.WriteLine($"{e.Message}\n{e.StackTrace}");
      Devices = [
        ..Devices.Where(device => device.Address != bluetoothDevice.Address),
        bluetoothDevice with { State = BluetoothConnectionState.Error }
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
      IReadOnlyCollection<BluetoothDevice> discoveredDevices = await Task.Run(bluetoothService.DiscoverDevices);
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