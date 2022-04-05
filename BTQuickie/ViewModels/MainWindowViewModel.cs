using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;
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
        private bool isConnected;

        public MainWindowViewModel(IBluetoothService bluetoothService)
        {
            this.bluetoothService = bluetoothService;
            this.devices = new List<BluetoothDeviceInfo>(bluetoothService.PairedDevices());
        }

        public IAsyncRelayCommand DiscoverDevicesCommand =>
            new AsyncRelayCommand(OnDiscoverBluetoothDevices, CanDiscoverBluetoothDevices);
        
        public IAsyncRelayCommand ConnectDisconnectCommand => new AsyncRelayCommand<string>(ConnectDisconnect);
        
        public IReadOnlyCollection<BluetoothDeviceInfo> Devices
        {
            get => this.devices;
            set
            {
                this.devices = value;
                OnPropertyChanged();
            }
        }

        public bool IsConnected
        {
            get => this.isConnected;
            set
            {
                this.isConnected = value;
                OnPropertyChanged();
            }
        }

        private async Task ConnectDisconnect(string? deviceAddress)
        {
            try
            {
                if (string.IsNullOrEmpty(deviceAddress))
                {
                    return;
                }

                if (this.bluetoothService.Connected)
                {
                    this.bluetoothService.Disconnect();
                    IsConnected = false;
                    return;
                }

                base.IsBusy = true;

                await Task.Run(() =>
                        this.bluetoothService.Connect(deviceAddress,
                            this.bluetoothService.GuidSerialPort()))
                    .WaitAsync(TimeSpan.FromMilliseconds(this.connectTimeoutMs));

                IsConnected = true;
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

        private void ToggleContextMenu()
        {
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