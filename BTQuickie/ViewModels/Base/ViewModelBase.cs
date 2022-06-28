using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BTQuickie.ViewModels.Base
{
    public partial class ViewModelBase : ObservableObject
    {
        [ObservableProperty]
        private bool isBusy;
        
        [ObservableProperty]
        private double windowWidth;
        
        [ObservableProperty]
        private double windowHeight;

        public virtual Task InitializeAsync()
        {
            return Task.CompletedTask;
        }
    }
}