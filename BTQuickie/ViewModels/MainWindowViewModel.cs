using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using BTQuickie.Services.Discovery;
using BTQuickie.ViewModels.Base;
using CommunityToolkit.Mvvm.Input;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;

namespace BTQuickie.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IBluetoothDiscoveryService bluetoothDiscoveryService;
        private IReadOnlyCollection<BluetoothDeviceInfo> devices;
        private BluetoothDeviceInfo selectedDevice;

        public MainWindowViewModel(IBluetoothDiscoveryService bluetoothDiscoveryService)
        {
            this.bluetoothDiscoveryService = bluetoothDiscoveryService;
            this.Devices = new List<BluetoothDeviceInfo>(bluetoothDiscoveryService.PairedDevices());
        }

        public ICommand DiscoverDevicesCommand =>
            new RelayCommand(OnDiscoverBluetoothDevices, CanDiscoverBluetoothDevices);

        public ICommand ConnectToDeviceCommand =>
            new AsyncRelayCommand<BluetoothDeviceInfo>(OnConnectToDevice);

        public BluetoothDeviceInfo SelectedDevice
        {
            get => this.selectedDevice;
            set
            {
                this.selectedDevice = value;
                base.IsBusy = true;
                _ = OnConnectToDevice(value);
                base.IsBusy = false;
                OnPropertyChanged();
            }
        }

        public IReadOnlyCollection<BluetoothDeviceInfo> Devices
        {
            get => this.devices;
            set
            {
                this.devices = value;
                OnPropertyChanged();
            }
        }

        private async Task OnConnectToDevice(BluetoothDeviceInfo? deviceInfo)
        {
            if (deviceInfo is null)
            {
                Debug.WriteLine($"{nameof(BluetoothDeviceInfo)} is null.");
                return;
            }

            await this.bluetoothDiscoveryService.ConnectAsync(deviceInfo.DeviceAddress, BluetoothService.GenericAudio);
        }

        private void OnDiscoverBluetoothDevices()
        {
            try
            {
                Debug.WriteLine($"Looking for devices...");

                new Thread(() =>
                {
                    base.IsBusy = true;

                    List<BluetoothDeviceInfo> devices = new(Devices);
                    devices.AddRange(this.bluetoothDiscoveryService.DiscoverDevices());
                    Devices = devices;

                    base.IsBusy = false;
                }).Start();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"EXCEPTION!\n{e.StackTrace}");
            }
        }

        private bool CanDiscoverBluetoothDevices()
        {
            return !base.IsBusy;
        }
    }
}