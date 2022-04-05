using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BluetoothDeviceInfoLocal = BTQuickie.Models.BluetoothDeviceInfo;

namespace BTQuickie.Services.Bluetooth
{
    public interface IBluetoothService
    {
        Guid GuidSerialPort();
        IReadOnlyCollection<BluetoothDeviceInfoLocal>  DiscoverDevices();
        void Connect(string address, Guid serviceGuid);
        Task ConnectAsync(string address, Guid serviceGuid);
        void PairRequest(string address, string pin);
        IEnumerable<BluetoothDeviceInfoLocal> PairedDevices();
        void Disconnect();
        bool Connected { get; }
    }
}