using System.Threading.Tasks;
using System.Windows;
using BTQuickie.Services.Application;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BTQuickie.ViewModels.Base
{
    public class ViewModelBase : ObservableObject
    {
        private bool isBusy;
        private double windowWidth;
        private double windowHeight;

        public double WindowWidth
        {
            get => this.windowWidth;
            set
            {
                this.windowWidth = value;
                OnPropertyChanged();
            }
        }

        public double WindowHeight
        {
            get => this.windowHeight;
            set
            {
                this.windowHeight = value;
                OnPropertyChanged();
            }
        }

        public bool IsBusy
        {
            get => this.isBusy;
            set
            {
                this.isBusy = value;
                OnPropertyChanged();
            }
        }

        public virtual Task InitializeAsync()
        {
         
            return Task.CompletedTask;
        }
    }
}