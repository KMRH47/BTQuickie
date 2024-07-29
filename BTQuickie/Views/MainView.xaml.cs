using System.ComponentModel;
using System.Windows;
using BTQuickie.ViewModels;

namespace BTQuickie.Views;

public partial class MainView
{
  public MainView(MainViewModel mainViewModel) {
    DataContext = mainViewModel;
    InitializeComponent();
  }

  protected override void OnClosing(CancelEventArgs e) {
    Application.Current.Shutdown();
  }
}