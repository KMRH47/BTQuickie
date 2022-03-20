using CommunityToolkit.Mvvm.ComponentModel;

namespace BTQuickie.ViewModels.Base
{
    public class ViewModelBase : ObservableObject
    {
        private bool isBusy;

        public bool IsBusy
        {
            get => this.isBusy;
            set
            {
                this.isBusy = value;
                OnPropertyChanged();
            }
        }
    }
}