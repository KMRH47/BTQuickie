using System.Diagnostics;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;

namespace BTQuickie.Services.Discovery
{
    public class BluetoothDiscoveryService : IBluetoothDiscoveryService
    {
        private readonly BluetoothListener bluetoothListener;
        
        public BluetoothDiscoveryService()
        {
            this.bluetoothListener = new BluetoothListener(BluetoothService.UPnpIPL2Cap);
        }

        public void Start()
        {
            Debug.WriteLine($"Starting Bluetooth discovery...");
            this.bluetoothListener.Start();
        }

        public void Stop()
        {
            Debug.WriteLine($"Stopping Bluetooth discovery...");
            this.bluetoothListener.Stop();
        }

        public bool IsActive => this.bluetoothListener.Active;
    }
}