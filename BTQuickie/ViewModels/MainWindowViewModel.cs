using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Input;
using BTQuickie.Services.Discovery;
using BTQuickie.ViewModels.Base;
using InTheHand.Net.Sockets;
using Prism.Commands;

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
        }

        public ICommand DiscoverDevicesCommand => new DelegateCommand(DiscoverBluetoothDevices, CanDiscoverBluetoothDevices);

        public BluetoothDeviceInfo SelectedDevice
        {
            get => this.selectedDevice;
            set
            {
                this.selectedDevice = value;
                Debug.WriteLine($"Selected: {value.DeviceName}");
                RaisePropertyChanged();
            }
        }

        public IReadOnlyCollection<BluetoothDeviceInfo> Devices
        {
            get => this.devices;
            set
            {
                this.devices = value;
                RaisePropertyChanged();
            }
        }

        private void DiscoverBluetoothDevices()
        {
            try
            {
                Debug.WriteLine($"Looking for devices...");

                new Thread(() =>
                {
                    base.IsBusy = true;

                    Devices= this.bluetoothDiscoveryService.DiscoverDevices();
                    
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