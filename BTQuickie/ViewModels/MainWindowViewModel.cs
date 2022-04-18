using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;
using BTQuickie.Models;
using BTQuickie.Services.Bluetooth;
using BTQuickie.Services.MainWindow;
using BTQuickie.ViewModels.Base;
using CommunityToolkit.Mvvm.Input;

namespace BTQuickie.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IBluetoothService bluetoothService;
        private readonly IMainWindowContextProvider mainWindowContextProvider;
        private IReadOnlyCollection<BluetoothDeviceInfo> devices;
        private BluetoothDeviceInfo selectedBluetoothDeviceInfo;
        private BluetoothDeviceInfo connectedBluetoothDeviceInfo;
        private int connectTimeoutMs = 5000;

        public MainWindowViewModel(IBluetoothService bluetoothService,
            IMainWindowContextProvider mainWindowContextProvider)
        {
            this.bluetoothService = bluetoothService;
            this.mainWindowContextProvider = mainWindowContextProvider;
            this.connectedBluetoothDeviceInfo = BluetoothDeviceInfo.Empty();
            this.devices = new List<BluetoothDeviceInfo>(bluetoothService.PairedDevices());
        }

        public IAsyncRelayCommand DiscoverDevicesCommand =>
            new AsyncRelayCommand(DiscoverBluetoothDevices, CanDiscoverBluetoothDevices);

        public IAsyncRelayCommand ConnectCommand => new AsyncRelayCommand<BluetoothDeviceInfo>(Connect);
        public IRelayCommand DisconnectCommand => new RelayCommand(Disconnect);
        public IRelayCommand ShowWindowCommand => new RelayCommand(ShowWindow);
        public IRelayCommand HideWindowCommand => new RelayCommand(HideWindow);
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

        public BluetoothDeviceInfo SelectedBluetoothDeviceInfo
        {
            get => this.selectedBluetoothDeviceInfo;
            set
            {
                this.selectedBluetoothDeviceInfo = value;
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

        private async Task Connect(BluetoothDeviceInfo bluetoothDeviceInfo)
        {
            try
            {
                base.IsBusy = true;
                
                if (ConnectedBluetoothDeviceInfo == bluetoothDeviceInfo)
                {
                    return;
                }

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
            this.mainWindowContextProvider.Close();
        }

        private async Task DiscoverBluetoothDevices()
        {
            base.IsBusy = true;

            Devices = await Task.Run(this.bluetoothService.DiscoverDevices);

            base.IsBusy = false;
        }

        private bool CanDiscoverBluetoothDevices()
        {
            return !base.IsBusy;
        }

        private void ShowWindow()
        {
            this.mainWindowContextProvider.Show();
        }

        private void HideWindow()
        {
            this.mainWindowContextProvider.Hide();
        }
    }
}