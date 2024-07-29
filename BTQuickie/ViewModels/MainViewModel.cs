using System.Collections.Generic;
using System.Collections.ObjectModel;
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
  [ObservableProperty] private BluetoothDeviceInfo connectedBluetoothDeviceInfo;

  [ObservableProperty] private IReadOnlyCollection<BluetoothDeviceInfo> devices =
    new ObservableCollection<BluetoothDeviceInfo>(bluetoothService.PairedDevices());

  [ObservableProperty] private BluetoothDeviceInfo selectedBluetoothDeviceInfo;

  [RelayCommand]
  private async Task ConnectAsync(BluetoothDeviceInfo bluetoothDeviceInfo) {
    try {
      IsBusy = true;

      if (ConnectedBluetoothDeviceInfo == bluetoothDeviceInfo) {
        return;
      }

      await bluetoothService.ConnectAsync(bluetoothDeviceInfo.Address, bluetoothService.GuidSerialPort());

      ConnectedBluetoothDeviceInfo = bluetoothDeviceInfo;
    }
    catch (SocketException e) {
      Debug.WriteLine($"{e.Message}\n{e.StackTrace}");
    }
    finally {
      IsBusy = false;
    }
  }


  [RelayCommand(CanExecute = nameof(CanDiscoverBluetoothDevices))]
  private async Task DiscoverBluetoothDevicesAsync()
  {
    IsBusy = true;
    try
    {
      IReadOnlyCollection<BluetoothDeviceInfo> discoveredDevices = await Task.Run(bluetoothService.DiscoverDevices);
      Devices = [..discoveredDevices, ..bluetoothService.PairedDevices()];
    }
    finally
    {
      IsBusy = false;
    }
  }

  [RelayCommand]
  private void Disconnect() {
    if (!bluetoothService.Connected) {
      return;
    }

    bluetoothService.Disconnect();
    ConnectedBluetoothDeviceInfo = default;
  }

  [RelayCommand]
  private void HideWindow() => applicationContextProvider.HideMainWindow();

  private bool CanDiscoverBluetoothDevices() => !IsBusy;
}