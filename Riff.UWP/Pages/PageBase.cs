using Riff.UWP.ViewModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System;

namespace Riff.UWP.Pages
{
    public abstract class PageBase : Page
    {
        private static readonly string LocatorKey = "VMLocator";
        public bool RegisterForChanges { get; protected set; } = false;
        public bool PreferViewUpdateBeforeLoad { get; protected set; } = false;
        public static Locator Locator => Windows.UI.Xaml.Application.Current.Resources[LocatorKey] as Locator;
        public abstract IDataViewModel DataViewModel { get; }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (RegisterForChanges)
            {
                DataViewModel.PropertyChanged += DataViewModel_PropertyChanged;
            }

            bool load = e.NavigationMode == NavigationMode.Refresh || !DataViewModel.IsLoaded;

            if (load)
            {
                if (PreferViewUpdateBeforeLoad)
                {
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async() => await DataViewModel.LoadAsync(e.Parameter ?? VoidType.Empty));
                }
                else
                {
                    await DataViewModel.LoadAsync(e.Parameter ?? VoidType.Empty);
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (RegisterForChanges)
            {
                DataViewModel.PropertyChanged -= DataViewModel_PropertyChanged;
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
