using System;
using System.Diagnostics;
using System.Windows.Input;
using BTQuickie.Services.Discovery;
using Prism.Commands;

namespace BTQuickie.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IBluetoothDiscoveryService bluetoothDiscoveryService;

        public MainWindowViewModel(IBluetoothDiscoveryService bluetoothDiscoveryService)
        {
            Debug.WriteLine($"Test: {bluetoothDiscoveryService}");
            this.bluetoothDiscoveryService = bluetoothDiscoveryService;
        }

        public ICommand BeginDiscoveringCommand => new DelegateCommand(BeginDiscovering, CanBeginDiscovering);

        private void BeginDiscovering()
        {
            try
            {
                this.bluetoothDiscoveryService.Start();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"EXCEPTION!\n{e.StackTrace}");
            }
        }

        private bool CanBeginDiscovering()
        {
            return !this.bluetoothDiscoveryService.IsActive;
        }
    }
}