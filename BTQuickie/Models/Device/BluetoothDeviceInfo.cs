namespace BTQuickie.Models.Device;

public record struct BluetoothDeviceInfo(string Name, string Address)
{
    public static BluetoothDeviceInfo Empty() => new(string.Empty, string.Empty);
}