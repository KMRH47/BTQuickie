using System;
using System.Windows;
using BTQuickie.Extensions;
using BTQuickie.Views;
using Microsoft.Extensions.DependencyInjection;

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
            MainWindow?.EnsureRendered();
        }

        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);
            MainWindow?.Hide();
        }
    }
}