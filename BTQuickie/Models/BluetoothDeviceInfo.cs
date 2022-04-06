namespace BTQuickie.Models;

public class BluetoothDeviceInfo
{
    public BluetoothDeviceInfo(string name, string address)
    {
        Name = name;
        Address = address;
    }

    public string Name { get; }
    public string Address { get; }

    public static BluetoothDeviceInfo Empty() => new(string.Empty, string.Empty);
}