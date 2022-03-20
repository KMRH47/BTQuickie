using System.Collections.Generic;
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

        public void Stop()
        {
        }

        public bool Connected => this.bluetoothClient.Connected;
    }
}