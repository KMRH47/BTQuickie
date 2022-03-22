using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BluetoothDeviceInfoLocal = BTQuickie.Models.BluetoothDeviceInfo;
using InTheHand.Net;
using InTheHand.Net.Sockets;

namespace BTQuickie.Services.Bluetooth
{
    public interface IBluetoothService
    {
        Guid GuidSerialPort();
        BluetoothAddress ParseBluetoothAddress(string address);
        IReadOnlyCollection<BluetoothDeviceInfoLocal>  DiscoverDevices();
        void Connect(BluetoothAddress bluetoothAddress, Guid serviceGuid);
        Task ConnectAsync(BluetoothEndPoint bluetoothEndPoint);
        Task ConnectAsync(BluetoothAddress bluetoothAddress, Guid serviceGuid);
        IEnumerable<BluetoothDeviceInfoLocal> PairedDevices();
        void Dispose();
        bool Connected { get; }
    }
}