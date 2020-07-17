using CommonServiceLocator;
using Mirage.ViewModel;
using Riff.UWP.ViewModel;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Riff.UWP.Pages
{
    public abstract class PageBase<TViewModel> : Page where TViewModel : IDataViewModel
    {
        static PageBase()
        {
            if (Windows.ApplicationModel.DesignMode.DesignMode2Enabled || Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                var bootstapper = new Bootstapper();
            }
        }
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

            OnLoad(e.NavigationMode);

            bool load = !ViewModel.IsLoaded || ViewModel.Parameter != (e.Parameter ?? VoidType.Empty);

            if (load)
            {
                if (PreferViewUpdateBeforeLoad)
                {
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async() => await ViewModel.LoadAsync(e.Parameter ?? VoidType.Empty));
                }
                else
                {
                    await ViewModel.LoadAsync(e.Parameter);
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

            OnUnload(e.NavigationMode);
        }

        protected virtual void OnLoad(NavigationMode mode)
        {
            
        }

        protected virtual void OnUnload(NavigationMode mode)
        {
            
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
