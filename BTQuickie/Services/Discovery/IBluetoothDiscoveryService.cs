using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InTheHand.Net;
using InTheHand.Net.Sockets;

namespace BTQuickie.Services.Discovery
{
    public interface IBluetoothDiscoveryService
    {
        IReadOnlyCollection<BluetoothDeviceInfo>  DiscoverDevices();
        Task ConnectAsync(BluetoothEndPoint bluetoothEndPoint);
        Task ConnectAsync(BluetoothAddress bluetoothAddress, Guid serviceGuid);
        IEnumerable<BluetoothDeviceInfo> PairedDevices();
        void Dispose();
        bool Connected { get; }
    }
}