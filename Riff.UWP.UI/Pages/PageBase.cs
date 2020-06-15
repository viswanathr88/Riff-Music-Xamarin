using CommonServiceLocator;
using Riff.UWP.ViewModel;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Riff.UWP.Pages
{
    public abstract class PageBase<TViewModel> : Page where TViewModel : IDataViewModel
    {
        public bool RegisterForChanges { get; protected set; } = false;
        public bool PreferViewUpdateBeforeLoad { get; protected set; } = false;
        public TViewModel ViewModel { get; } = ServiceLocator.Current.GetInstance<TViewModel>();

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (RegisterForChanges)
            {
                ViewModel.PropertyChanged += DataViewModel_PropertyChanged;
            }

            bool load = !ViewModel.IsLoaded;

            if (load)
            {
                if (PreferViewUpdateBeforeLoad)
                {
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async() => await ViewModel.LoadAsync(e.Parameter ?? VoidType.Empty));
                }
                else
                {
                    await ViewModel.LoadAsync(e.Parameter ?? VoidType.Empty);
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (RegisterForChanges)
            {
                ViewModel.PropertyChanged -= DataViewModel_PropertyChanged;
            }
        }

        private void DataViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            HandleViewModelPropertyChanged(e.PropertyName);
        }

        protected virtual void HandleViewModelPropertyChanged(string propertyName)
        {
        }
    }
}
