namespace BTQuickie.Models.Device;

public record struct BluetoothDeviceInfo(
  string Name,
  string Address,
  BluetoothConnectionState State
);

public enum BluetoothConnectionState
{
  Discovered,
  Connected,
  Paired,
  Error,
}
