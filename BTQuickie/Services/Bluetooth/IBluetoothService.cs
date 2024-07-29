using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BluetoothDeviceInfoLocal = BTQuickie.Models.Device.BluetoothDeviceInfo;

namespace BTQuickie.Services.Bluetooth;

public interface IBluetoothService
{
  bool Connected { get; }
  Guid GuidSerialPort();
  IReadOnlyCollection<BluetoothDeviceInfoLocal> DiscoverDevices();
  void Connect(string address, Guid serviceGuid);
  Task ConnectAsync(string address, Guid serviceGuid);
  void PairRequest(string address, string pin);
  IEnumerable<BluetoothDeviceInfoLocal> PairedDevices();
  void Disconnect();
}