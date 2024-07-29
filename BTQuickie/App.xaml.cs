using System;
using System.Windows;
using BTQuickie.Extensions;
using BTQuickie.Services.Settings;
using BTQuickie.ViewModels.Base;
using BTQuickie.Views;
using Microsoft.Extensions.DependencyInjection;

namespace BTQuickie;

public partial class App
{
  private readonly IApplicationSettingsProvider applicationSettingsProvider;
  private readonly IServiceProvider serviceProvider;

  public App() {
    IServiceProvider serviceProvider = new ServiceCollection()
      .ConfigureServices()
      .ConfigureViewModels()
      .ConfigureViews()
      .BuildServiceProvider();

    applicationSettingsProvider = serviceProvider.GetRequiredService<IApplicationSettingsProvider>();
    this.serviceProvider = serviceProvider;
  }

  protected override void OnDeactivated(EventArgs e) {
    base.OnDeactivated(e);
    MainWindow?.Hide();
    applicationSettingsProvider.WriteUserSettings();
  }

  private void OnStartup(object sender, StartupEventArgs e) {
    applicationSettingsProvider.WriteUserSettings();
    var taskbarIconView = serviceProvider.GetRequiredService<TaskbarIconView>();
    var settingsView = serviceProvider.GetRequiredService<SettingsView>();
    MainWindow = serviceProvider.GetRequiredService<MainView>();
    taskbarIconView.EnsureHandle();
    settingsView.EnsureHandle();
    MainWindow.EnsureHandle();

    foreach (Window window in Current.Windows)
      window.Activated += OnWindowActivated;
  }

  private static void OnWindowActivated(object? o, EventArgs e) {
    var window = (Window)o!;
    var viewModelBase = (ViewModelBase)window.DataContext;
    viewModelBase.InitializeAsync();
  }

  protected override void OnExit(ExitEventArgs e) {
    base.OnExit(e);
    applicationSettingsProvider.WriteUserSettings();
  }
}