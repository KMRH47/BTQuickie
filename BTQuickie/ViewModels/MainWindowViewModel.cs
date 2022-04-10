using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;
using BTQuickie.Models;
using BTQuickie.Services.Application;
using BTQuickie.Services.Bluetooth;
using BTQuickie.ViewModels.Base;
using CommunityToolkit.Mvvm.Input;

namespace BTQuickie.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IBluetoothService bluetoothService;
        private readonly IMainWindowContextProvider appContextProvider;
        private IReadOnlyCollection<BluetoothDeviceInfo> devices;
        private BluetoothDeviceInfo connectedBluetoothDeviceInfo;
        private int connectTimeoutMs = 5000;

        public MainWindowViewModel(IBluetoothService bluetoothService, IMainWindowContextProvider appContextProvider)
        {
            this.bluetoothService = bluetoothService;
            this.appContextProvider = appContextProvider;
            this.connectedBluetoothDeviceInfo = BluetoothDeviceInfo.Empty();
            this.devices = new List<BluetoothDeviceInfo>(bluetoothService.PairedDevices());
        }

        public IAsyncRelayCommand DiscoverDevicesCommand =>
            new AsyncRelayCommand(OnDiscoverBluetoothDevices, CanDiscoverBluetoothDevices);

        public IAsyncRelayCommand ConnectCommand => new AsyncRelayCommand<BluetoothDeviceInfo>(Connect);
        public IRelayCommand DisconnectCommand => new RelayCommand(Disconnect);
        public IRelayCommand ToggleWindowVisibleCommand => new RelayCommand(ToggleWindowVisible);
        public IRelayCommand ExitCommand => new RelayCommand(Exit);

        public IReadOnlyCollection<BluetoothDeviceInfo> Devices
        {
            get => this.devices;
            set
            {
                this.devices = value;
                OnPropertyChanged();
            }
        }

        public BluetoothDeviceInfo ConnectedBluetoothDeviceInfo
        {
            get => this.connectedBluetoothDeviceInfo;
            set
            {
                this.connectedBluetoothDeviceInfo = value;
                OnPropertyChanged();
            }
        }

        private async Task Connect(BluetoothDeviceInfo? bluetoothDeviceInfo)
        {
            try
            {
                if (bluetoothDeviceInfo is null)
                {
                    return;
                }

                base.IsBusy = true;

                await Task.Run(() =>
                        this.bluetoothService.Connect(bluetoothDeviceInfo.Address,
                            this.bluetoothService.GuidSerialPort()))
                    .WaitAsync(TimeSpan.FromMilliseconds(this.connectTimeoutMs));

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

        private void Disconnect()
        {
            if (!this.bluetoothService.Connected)
            {
                return;
            }

            this.bluetoothService.Disconnect();
            ConnectedBluetoothDeviceInfo = BluetoothDeviceInfo.Empty();
        }
        private void Exit()
        {
            this.bluetoothService.Disconnect();
            this.appContextProvider.Close();
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

        private void ToggleWindowVisible()
        {
            this.appContextProvider.Show();
        }
    }
}