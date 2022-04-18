using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BluetoothDeviceInfoLocal = BTQuickie.Models.BluetoothDeviceInfo;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;

namespace BTQuickie.Services.Bluetooth
{
    public class InTheHandBluetoothService : IBluetoothService
    {
        private BluetoothClient bluetoothClient;

        public InTheHandBluetoothService()
        {
            this.bluetoothClient = CreateClient();
        }

        public bool Connected => this.bluetoothClient.Connected;

        public Guid GuidSerialPort()
        {
            return BluetoothService.SerialPort;
        }

        public IReadOnlyCollection<BluetoothDeviceInfoLocal> DiscoverDevices()
        {
            return MapModel(this.bluetoothClient.DiscoverDevices());
        }

        public IEnumerable<BluetoothDeviceInfoLocal> PairedDevices()
        {
            return MapModel(this.bluetoothClient.PairedDevices);
        }

        public void Connect(string address, Guid serviceGuid)
        {
            this.bluetoothClient.Connect(BluetoothAddress.Parse(address), serviceGuid);
        }

        public async Task ConnectAsync(string address, Guid serviceGuid)
        {
            await this.bluetoothClient.ConnectAsync(BluetoothAddress.Parse(address), serviceGuid);
        }

        public void PairRequest(string address, string pin)
        {
            BluetoothSecurity.PairRequest(BluetoothAddress.Parse(address), pin);
        }

        public void Disconnect()
        {
           this.bluetoothClient.Dispose();
           this.bluetoothClient = CreateClient();
        }

        private static IReadOnlyCollection<BluetoothDeviceInfoLocal> MapModel(IEnumerable<BluetoothDeviceInfo> devices)
        {
            return devices.Select(bluetoothDeviceInfo =>
                    new BluetoothDeviceInfoLocal(
                        bluetoothDeviceInfo.DeviceName,
                        bluetoothDeviceInfo.DeviceAddress.ToString()))
                .ToList();
        }

        private BluetoothClient CreateClient() => new() {InquiryLength = TimeSpan.FromSeconds(3)};
    }
}