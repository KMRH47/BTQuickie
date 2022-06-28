using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;
using BTQuickie.Models.Device;
using BTQuickie.Services.Application;
using BTQuickie.Services.Bluetooth;
using BTQuickie.ViewModels.Base;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BTQuickie.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        private readonly IApplicationContextProvider applicationContextProvider;
        private readonly IBluetoothService bluetoothService;
        
        [ObservableProperty]
        private IReadOnlyCollection<BluetoothDeviceInfo> devices;
        
        [ObservableProperty]
        private BluetoothDeviceInfo selectedBluetoothDeviceInfo;
        
        [ObservableProperty]
        private BluetoothDeviceInfo connectedBluetoothDeviceInfo;
     
        public MainViewModel(IBluetoothService bluetoothService, IApplicationContextProvider applicationContextProvider)
        {
            this.applicationContextProvider = applicationContextProvider;
            this.bluetoothService = bluetoothService;
            this.connectedBluetoothDeviceInfo = BluetoothDeviceInfo.Empty();
            this.devices = new ObservableCollection<BluetoothDeviceInfo>(bluetoothService.PairedDevices());
        }

        [RelayCommand]
        private async Task ConnectAsync(BluetoothDeviceInfo bluetoothDeviceInfo)
        {
            try
            {
                base.IsBusy = true;

                if (ConnectedBluetoothDeviceInfo == bluetoothDeviceInfo)
                {
                    return;
                }

                await this.bluetoothService.ConnectAsync(bluetoothDeviceInfo.Address,
                                                         this.bluetoothService.GuidSerialPort());

                ConnectedBluetoothDeviceInfo = bluetoothDeviceInfo;
            }
            catch (SocketException e)
            {
                Debug.WriteLine($"{e.Message}\n{e.StackTrace}");
            }
            finally
            {
                base.IsBusy = false;
            }
        }
        
        [RelayCommand(CanExecute = nameof(CanDiscoverBluetoothDevices))]
        private async Task DiscoverBluetoothDevicesAsync()
        {
            base.IsBusy = true;

            Devices = await Task.Run(this.bluetoothService.DiscoverDevices);

            base.IsBusy = false;
        }

        [RelayCommand]
        private void Disconnect()
        {
            if (!this.bluetoothService.Connected)
            {
                return;
            }

            this.bluetoothService.Disconnect();
            ConnectedBluetoothDeviceInfo = BluetoothDeviceInfo.Empty();
        }
        
        [RelayCommand]
        private void HideWindow()
        {
            this.applicationContextProvider.HideMainWindow();
        }

        private bool CanDiscoverBluetoothDevices()
        {
            return !base.IsBusy;
        }
    }
}