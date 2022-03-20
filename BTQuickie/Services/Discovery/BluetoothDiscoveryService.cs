using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InTheHand.Net;
using InTheHand.Net.Sockets;

namespace BTQuickie.Services.Discovery
{
    public class BluetoothDiscoveryService : IBluetoothDiscoveryService
    {
        private readonly BluetoothClient bluetoothClient = new();
        //    private readonly BluetoothListener bluetoothListener = new(BluetoothService.SerialPort);

        public IReadOnlyCollection<BluetoothDeviceInfo> DiscoverDevices()
        {
            return this.bluetoothClient.DiscoverDevices();
        }

        public Task ConnectAsync(BluetoothEndPoint bluetoothEndPoint)
        {
            return this.bluetoothClient.ConnectAsync(bluetoothEndPoint);
        }

        public Task ConnectAsync(BluetoothAddress bluetoothAddress, Guid serviceGuid)
        {
            return this.bluetoothClient.ConnectAsync(bluetoothAddress, serviceGuid);
        }

        public IEnumerable<BluetoothDeviceInfo> PairedDevices()
        {
            return this.bluetoothClient.PairedDevices;
        }

        public void Dispose()
        {
            this.bluetoothClient.Dispose();
        }

        public bool Connected => this.bluetoothClient.Connected;
    }
}