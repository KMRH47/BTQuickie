namespace BTQuickie.Models.Device;

public record struct BluetoothDeviceInfo(string Name, string Address)
{
  public static BluetoothDeviceInfo Empty() {
    return new BluetoothDeviceInfo(string.Empty, string.Empty);
  }
}