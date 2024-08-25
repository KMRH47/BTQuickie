using System.Collections.Generic;
using System.Threading.Tasks;
using BTQuickie.Models.Device;

namespace BTQuickie.Services.Bluetooth;

public interface IBluetoothService
{
  bool Connected { get; }
  Task<IReadOnlyCollection<BluetoothDevice>> DiscoverDevices();
  Task ConnectAsync(BluetoothDevice bluetoothDevice);
  void PairRequest(BluetoothDevice bluetoothDevice, string? pin = null);
  IReadOnlyCollection<BluetoothDevice> GetPairedDevices();
  void Disconnect();
}