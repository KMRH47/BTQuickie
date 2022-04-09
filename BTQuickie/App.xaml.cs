using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using BTQuickie.Extensions;
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
        }

        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);
            MainWindow?.Hide();
        }
    }
}