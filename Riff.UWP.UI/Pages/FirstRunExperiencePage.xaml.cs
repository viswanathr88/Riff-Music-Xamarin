using CommonServiceLocator;
using Riff.UWP.ViewModel;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Riff.UWP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FirstRunExperiencePage : Page
    {
        public FirstRunExperienceViewModel ViewModel => ServiceLocator.Current.GetInstance<FirstRunExperienceViewModel>();

        public FirstRunExperiencePage()
        {
            this.InitializeComponent();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            // base.OnNavigatedTo(e);
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;

            await ViewModel.LoadAsync();

            // Check the value of LoginRequired now
            if (!ViewModel.LoginRequired)
            {
                // Navigate to Main Page
                Frame.Navigate(typeof(MainPage), null , new EntranceNavigationTransitionInfo());
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.ProfileAndPhotoFetched) && ViewModel.ProfileAndPhotoFetched)
            {
                // Navigate to Main Page
                Frame.Navigate(typeof(MainPage), null, new EntranceNavigationTransitionInfo());
            }
        }

    }
}
