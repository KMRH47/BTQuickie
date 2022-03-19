namespace BTQuickie.Services.Discovery
{
    public interface IBluetoothDiscoveryService
    {
        void Start();
        void Stop();
        bool IsActive { get; }
    }
}