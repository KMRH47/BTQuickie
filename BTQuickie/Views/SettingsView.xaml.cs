using System.ComponentModel;
using BTQuickie.ViewModels;

namespace BTQuickie.Views;

public partial class SettingsView
{
  public SettingsView(SettingsViewModel settingsViewModel) {
    DataContext = settingsViewModel;
    InitializeComponent();
  }

  private void SettingsView_OnClosing(object? sender, CancelEventArgs e) {
    e.Cancel = true;
    Hide();
  }
}