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
        private readonly BluetoothClient bluetoothClient = new() {InquiryLength = TimeSpan.FromSeconds(3)};

        public Guid GuidSerialPort()
        {
            return BluetoothService.SerialPort;
        }

        public IReadOnlyCollection<BluetoothDeviceInfoLocal> DiscoverDevices()
        {
            IReadOnlyCollection<BluetoothDeviceInfo>? discoveredDevices = this.bluetoothClient.DiscoverDevices();
            return MapModel(discoveredDevices);
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

        public IEnumerable<BluetoothDeviceInfoLocal> PairedDevices()
        {
            IEnumerable<BluetoothDeviceInfo>? pairedDevices = this.bluetoothClient.PairedDevices;
            return MapModel(pairedDevices);
        }

        public void Dispose()
        {
            this.bluetoothClient.Dispose();
        }

        public bool Connected => this.bluetoothClient.Connected;

        private static IReadOnlyCollection<BluetoothDeviceInfoLocal> MapModel(IEnumerable<BluetoothDeviceInfo> devices)
        {
            return devices.Select(deviceInfo =>
                new BluetoothDeviceInfoLocal(deviceInfo.DeviceName, deviceInfo.DeviceAddress.ToString())).ToList();
        }
    }
}