using System.Collections.Generic;
using System.Threading.Tasks;
using BTQuickie.Models.Device;

namespace BTQuickie.Services.Bluetooth;

public interface IBluetoothService
{
  bool Connected { get; }
  Task<IReadOnlyCollection<BluetoothDeviceInfo>> DiscoverDevices();
  Task ConnectAsync(BluetoothDeviceInfo bluetoothDeviceInfo);
  void PairRequest(BluetoothDeviceInfo bluetoothDeviceInfo, string? pin = null);
  IReadOnlyCollection<BluetoothDeviceInfo> GetPairedDevices();
  void Disconnect();
}