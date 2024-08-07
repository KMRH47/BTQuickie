﻿using BTQuickie.Services.Application;
using BTQuickie.Services.Bluetooth;
using BTQuickie.Services.Settings;
using BTQuickie.ViewModels;
using BTQuickie.ViewModels.Base;
using BTQuickie.Views;
using Microsoft.Extensions.DependencyInjection;

namespace BTQuickie.Extensions;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection ConfigureServices(this IServiceCollection services) {
    return services
      .AddScoped<IBluetoothService, InTheHandBluetoothService>()
      .AddScoped<IApplicationSettingsProvider, ApplicationSettingsProvider>()
      .AddScoped<IApplicationContextProvider, ApplicationContextProvider>();
  }

  public static IServiceCollection ConfigureViews(this IServiceCollection services) {
    return services
      .AddSingleton<MainView>()
      .AddSingleton<SettingsView>()
      .AddSingleton<TaskbarIconView>();
  }

  public static IServiceCollection ConfigureViewModels(this IServiceCollection services) {
    return services
      .AddSingleton<ViewModelBase>()
      .AddSingleton<MainViewModel>()
      .AddSingleton<SettingsViewModel>()
      .AddSingleton<TaskbarIconViewModel>();
  }
}