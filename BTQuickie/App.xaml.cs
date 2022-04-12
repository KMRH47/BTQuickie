using System;
using System.Windows;
using System.Windows.Media.Imaging;
using BTQuickie.Extensions;
using BTQuickie.Resources.Localization;
using BTQuickie.ViewModels;
using BTQuickie.Views;
using Microsoft.Extensions.DependencyInjection;
using NullSoftware.ToolKit;

namespace BTQuickie
{
    public partial class App
    {
        private readonly IServiceProvider serviceProvider;

        public App()
        {
            this.serviceProvider = new ServiceCollection()
                .ConfigureServices()
                .ConfigureViews()
                .ConfigureViewModels()
                .BuildServiceProvider();
        }
        

        private void OnStartup(object sender, StartupEventArgs e)
        {
            MainWindow = this.serviceProvider.GetService<MainWindow>();
            
            if (MainWindow is null)
            {
                return;
            }
            
            MainWindow.ShowMinimal();
        }

        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);
            MainWindow?.Hide();
        }
    }
}