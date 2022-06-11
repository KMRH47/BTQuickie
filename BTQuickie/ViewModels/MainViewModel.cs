using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;
using BTQuickie.Models.Device;
using BTQuickie.Services.Application;
using BTQuickie.Services.Bluetooth;
using BTQuickie.ViewModels.Base;
using CommunityToolkit.Mvvm.Input;

namespace BTQuickie.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IApplicationContextProvider applicationContextProvider;
        private readonly IBluetoothService bluetoothService;
        private IReadOnlyCollection<BluetoothDeviceInfo> devices;
        private BluetoothDeviceInfo selectedBluetoothDeviceInfo;
        private BluetoothDeviceInfo connectedBluetoothDeviceInfo;

        public MainViewModel(IBluetoothService bluetoothService, IApplicationContextProvider applicationContextProvider)
        {
            this.applicationContextProvider = applicationContextProvider;
            this.bluetoothService = bluetoothService;
            this.connectedBluetoothDeviceInfo = BluetoothDeviceInfo.Empty();
            this.devices = new ObservableCollection<BluetoothDeviceInfo>(bluetoothService.PairedDevices());
        }

        public IAsyncRelayCommand DiscoverDevicesCommand =>
            new AsyncRelayCommand(DiscoverBluetoothDevicesAsync, CanDiscoverBluetoothDevices);

        public IAsyncRelayCommand ConnectCommand => new AsyncRelayCommand<BluetoothDeviceInfo>(ConnectAsync);
        public IRelayCommand DisconnectCommand => new RelayCommand(Disconnect);
        public IRelayCommand HideWindowCommand => new RelayCommand(HideWindow);

        public BluetoothDeviceInfo SelectedBluetoothDeviceInfo
        {
            get => this.selectedBluetoothDeviceInfo;
            set
            {
                this.selectedBluetoothDeviceInfo = value;
                OnPropertyChanged();
            }
        }

        public IReadOnlyCollection<BluetoothDeviceInfo> Devices
        {
            get => this.devices;
            private set
            {
                this.devices = value;
                OnPropertyChanged();
            }
        }

        public BluetoothDeviceInfo ConnectedBluetoothDeviceInfo
        {
            get => this.connectedBluetoothDeviceInfo;
            private set
            {
                this.connectedBluetoothDeviceInfo = value;
                OnPropertyChanged();
            }
        }

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

        private void Disconnect()
        {
            if (!this.bluetoothService.Connected)
            {
                return;
            }

            this.bluetoothService.Disconnect();
            ConnectedBluetoothDeviceInfo = BluetoothDeviceInfo.Empty();
        }

        private async Task DiscoverBluetoothDevicesAsync()
        {
            base.IsBusy = true;

            Devices = await Task.Run(this.bluetoothService.DiscoverDevices);

            base.IsBusy = false;
        }

        private bool CanDiscoverBluetoothDevices()
        {
            return !base.IsBusy;
        }

        private void HideWindow()
        {
            this.applicationContextProvider.HideMainWindow();
        }
    }
}