using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BTQuickie.ViewModels.Base;

public partial class ViewModelBase : ObservableObject
{
  [ObservableProperty] private bool isBusy;

  [ObservableProperty] private double windowHeight;

  [ObservableProperty] private double windowWidth;

  public virtual Task InitializeAsync() {
    return Task.CompletedTask;
  }
}