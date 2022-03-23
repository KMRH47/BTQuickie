using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using BTQuickie.Models;
using BTQuickie.Services.Bluetooth;
using BTQuickie.ViewModels.Base;
using CommunityToolkit.Mvvm.Input;

namespace BTQuickie.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IBluetoothService bluetoothService;
        private IReadOnlyCollection<BluetoothDeviceInfo> devices;
        private int connectTimeoutMs = 5000;
        private bool showPairedDevices = true;

        public MainWindowViewModel(IBluetoothService bluetoothService)
        {
            this.bluetoothService = bluetoothService;
            this.devices = new List<BluetoothDeviceInfo>(this.bluetoothService.PairedDevices());
        }

        public ICommand DiscoverDevicesCommand =>
            new AsyncRelayCommand(OnDiscoverBluetoothDevices, CanDiscoverBluetoothDevices);

        public ICommand ConnectToDeviceCommand =>
            new AsyncRelayCommand<string>(OnConnectToDevice);

        public IReadOnlyCollection<BluetoothDeviceInfo> Devices
        {
            get => this.devices;
            set
            {
                this.devices = value;
                OnPropertyChanged();
            }
        }

        private async Task OnConnectToDevice(string? deviceAddress)
        {
            if (deviceAddress is null or "")
            {
                return;
            }

            base.IsBusy = true;

            await Task.Run(() =>
                    this.bluetoothService.Connect(deviceAddress,
                        this.bluetoothService.GuidSerialPort()))
                .WaitAsync(TimeSpan.FromMilliseconds(this.connectTimeoutMs));

            base.IsBusy = false;
        }

        private async Task OnDiscoverBluetoothDevices()
        {
            base.IsBusy = true;

            Devices = await Task.Run(() => _ = this.bluetoothService.DiscoverDevices());

            base.IsBusy = false;
        }

        private bool CanDiscoverBluetoothDevices()
        {
            return !base.IsBusy;
        }
    }
}