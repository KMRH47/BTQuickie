namespace BTQuickie.Models.Device;

public record struct BluetoothDeviceInfo(string Name = "", string Address = "", bool IsPaired = false);