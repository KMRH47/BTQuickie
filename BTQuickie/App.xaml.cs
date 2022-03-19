﻿using System;
using System.Diagnostics;
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
            Debug.Write("OnStartup");
            (MainWindow = this.serviceProvider.GetService<MainWindow>())?.Show();
        }
    }
}