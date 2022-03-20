using System.Collections.Generic;
using System.Threading.Tasks;
using InTheHand.Net.Sockets;

namespace BTQuickie.Services.Discovery
{
    public interface IBluetoothDiscoveryService
    {
        IReadOnlyCollection<BluetoothDeviceInfo>  DiscoverDevices();
        bool Connected { get; }
    }
}