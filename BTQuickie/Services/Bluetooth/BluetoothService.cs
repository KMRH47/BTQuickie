using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BluetoothDeviceInfoLocal = BTQuickie.Models.BluetoothDeviceInfo;
using InTheHand.Net;
using InTheHand.Net.Sockets;

namespace BTQuickie.Services.Bluetooth
{
    public class BluetoothService : IBluetoothService
    {
        private readonly BluetoothClient bluetoothClient = new() {InquiryLength = TimeSpan.FromSeconds(3)};

        public Guid GuidSerialPort()
        {
            return InTheHand.Net.Bluetooth.BluetoothService.SerialPort;
        }

        public BluetoothAddress ParseBluetoothAddress(string address)
        {
            return BluetoothAddress.Parse(address);
        }

        public IReadOnlyCollection<BluetoothDeviceInfoLocal> DiscoverDevices()
        {
            IReadOnlyCollection<BluetoothDeviceInfo>? discoveredDevices = this.bluetoothClient.DiscoverDevices();
            return MapModel(discoveredDevices);
        }

        public void Connect(BluetoothAddress bluetoothAddress, Guid serviceGuid)
        {
            this.bluetoothClient.Connect(bluetoothAddress, serviceGuid);
        }

        public async Task ConnectAsync(BluetoothEndPoint bluetoothEndPoint)
        {
            await this.bluetoothClient.ConnectAsync(bluetoothEndPoint);
        }

        public Task ConnectAsync(BluetoothAddress bluetoothAddress, Guid serviceGuid)
        {
            return this.bluetoothClient.ConnectAsync(bluetoothAddress, serviceGuid);
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