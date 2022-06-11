using System;
using System.Windows;
using BTQuickie.Extensions;
using BTQuickie.Services.Settings;
using BTQuickie.ViewModels.Base;
using BTQuickie.Views;
using Microsoft.Extensions.DependencyInjection;

namespace BTQuickie
{
    public partial class App
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IApplicationSettingsProvider applicationSettingsProvider;

        public App()
        {
            IServiceProvider serviceProvider = new ServiceCollection()
                                              .ConfigureServices()
                                              .ConfigureViewModels()
                                              .ConfigureViews()
                                              .BuildServiceProvider();
            
            this.applicationSettingsProvider = serviceProvider.GetRequiredService<IApplicationSettingsProvider>();
            this.serviceProvider = serviceProvider;
        }

        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);
            MainWindow?.Hide();
            this.applicationSettingsProvider.WriteUserSettings();
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            this.applicationSettingsProvider.WriteUserSettings();
            TaskbarIconView taskbarIconView = this.serviceProvider.GetRequiredService<TaskbarIconView>();
            SettingsView settingsView = this.serviceProvider.GetRequiredService<SettingsView>();
            MainWindow = this.serviceProvider.GetRequiredService<MainView>();
            taskbarIconView.EnsureHandle();
            settingsView.EnsureHandle();
            MainWindow.EnsureHandle();

            foreach (Window window in Current.Windows)
            {
                window.Activated += OnWindowActivated;
            }
        }

        private static void OnWindowActivated(object? o, EventArgs e)
        {
            Window window = (Window) o!;
            ViewModelBase viewModelBase = (ViewModelBase) window.DataContext;
            viewModelBase.InitializeAsync();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            this.applicationSettingsProvider.WriteUserSettings();
        }
    }
}