using BTQuickie.Services.Discovery;
using BTQuickie.ViewModels;
using BTQuickie.ViewModels.Base;
using BTQuickie.Views;
using Microsoft.Extensions.DependencyInjection;

namespace BTQuickie.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureServices(this IServiceCollection services)
    {
        return services
            .AddScoped<IBluetoothDiscoveryService, BluetoothDiscoveryService>();
    }

    public static IServiceCollection ConfigureViewModels(this IServiceCollection services)
    {
        return services
            .AddSingleton<MainWindowViewModel>()
            .AddSingleton<ViewModelBase>();
    }

    public static IServiceCollection ConfigureViews(this IServiceCollection services)
    {
        return services
            .AddSingleton<MainWindow>();
    }
}