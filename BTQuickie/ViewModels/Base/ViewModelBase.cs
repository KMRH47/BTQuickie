using Prism.Mvvm;

namespace BTQuickie.ViewModels.Base
{
    public class ViewModelBase : BindableBase
    {
        private bool isBusy;

        public bool IsBusy
        {
            get => this.isBusy;
            set
            {
                this.isBusy = value;
                RaisePropertyChanged();
            }
        }
    }
}