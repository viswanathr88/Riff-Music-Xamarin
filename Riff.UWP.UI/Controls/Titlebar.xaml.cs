using Microsoft.UI.Xaml.Controls;
using System;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Riff.UWP.Controls
{
    public sealed partial class Titlebar : UserControl
    {
        CoreApplicationViewTitleBar coreTitlebar = CoreApplication.GetCurrentView().TitleBar;

        public Titlebar()
        {
            this.InitializeComponent();
            Loaded += Titlebar_Loaded;
            Unloaded += Titlebar_Unloaded;
            Background = (Brush)Application.Current.Resources["NavigationViewTopPaneBackground"];
        }

        public event EventHandler<EventArgs> BackRequested;

        public string AppName
        {
            get { return (string)GetValue(AppNameProperty); }
            set { SetValue(AppNameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AppName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AppNameProperty =
            DependencyProperty.Register("AppName", typeof(string), typeof(Titlebar), new PropertyMetadata(string.Empty));

        public bool IsBackButtonVisible
        {
            get { return (bool)GetValue(IsBackButtonVisibleProperty); }
            set { SetValue(IsBackButtonVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsBackButtonVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsBackButtonVisibleProperty =
            DependencyProperty.Register("IsBackButtonVisible", typeof(bool), typeof(Titlebar), new PropertyMetadata(false, OnIsBackButtonVisibleChanged));

        public double CoreTitlebarHeight
        {
            get { return (double)GetValue(CoreTitlebarHeightProperty); }
            set { SetValue(CoreTitlebarHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CoreTitlebarHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CoreTitlebarHeightProperty =
            DependencyProperty.Register("CoreTitlebarHeight", typeof(double), typeof(Titlebar), new PropertyMetadata(0.0));

        public Thickness CoreTitlebarPadding
        {
            get { return (Thickness)GetValue(CoreTitlebarPaddingProperty); }
            set { SetValue(CoreTitlebarPaddingProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CoreTitlebarLeftPadding.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CoreTitlebarPaddingProperty =
            DependencyProperty.Register("CoreTitlebarPadding", typeof(Thickness), typeof(Titlebar), new PropertyMetadata(new Thickness(0,0,0,0)));

        public Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode PaneDisplayMode
        {
            get { return (Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode)GetValue(PaneDisplayModeProperty); }
            set { SetValue(PaneDisplayModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PaneDisplayMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PaneDisplayModeProperty =
            DependencyProperty.Register("PaneDisplayMode", typeof(Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode), typeof(Titlebar), new PropertyMetadata(Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Minimal, OnNavigationViewModeChanged));

        public bool IsPaneOpen
        {
            get { return (bool)GetValue(IsPaneOpenProperty); }
            set { SetValue(IsPaneOpenProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsPaneVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsPaneOpenProperty =
            DependencyProperty.Register("IsPaneOpen", typeof(bool), typeof(Titlebar), new PropertyMetadata(false, OnIsPaneOpenChanged));

        private void Titlebar_Loaded(object sender, RoutedEventArgs e)
        {
            coreTitlebar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;
            coreTitlebar.IsVisibleChanged += CoreTitlebar_IsVisibleChanged;

            Window.Current.SetTitleBar(TitlebarMiddleGrid);

            UpdateLayoutMetrics();
            UpdateVisibility();
        }

        private void UpdateVisibility()
        {
            Visibility = coreTitlebar.IsVisible ? Visibility.Visible : Visibility.Collapsed;
            if (UIViewSettings.GetForCurrentView().UserInteractionMode == UserInteractionMode.Touch && coreTitlebar.IsVisible)
            {
                Background = Application.Current.Resources["SystemControlBackgroundAccentBrush"] as Brush;
            }
            else
            {
                Background = null;
            }
        }

        private void CoreTitlebar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
        {
            UpdateVisibility();
        }

        private void Titlebar_Unloaded(object sender, RoutedEventArgs e)
        {
            coreTitlebar.LayoutMetricsChanged -= CoreTitleBar_LayoutMetricsChanged;
        }

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            UpdateLayoutMetrics();
        }

        private void UpdateLayoutMetrics()
        {
            if (coreTitlebar.Height > 0)
            {
                this.CoreTitlebarHeight = coreTitlebar.Height;
                this.SetValue(Grid.RowProperty, 1);
                Background = null;
            }

            // The SystemOverlayLeftInset and SystemOverlayRightInset values are
            // in terms of physical left and right. Therefore, we need to flip
            // then when our flow direction is RTL.
            if (FlowDirection == FlowDirection.LeftToRight)
            {
                this.CoreTitlebarPadding = new Thickness()
                {
                    Left = coreTitlebar.SystemOverlayLeftInset > 0 ? coreTitlebar.SystemOverlayLeftInset : this.CoreTitlebarPadding.Left,
                    Right = coreTitlebar.SystemOverlayRightInset
                };
            }
            else
            {
                this.CoreTitlebarPadding = new Thickness()
                {
                    Left = coreTitlebar.SystemOverlayRightInset,
                    Right = coreTitlebar.SystemOverlayLeftInset > 0 ? coreTitlebar.SystemOverlayLeftInset : this.CoreTitlebarPadding.Left
                };
            }
        }

        private static void OnIsBackButtonVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool isVisible = (bool)e.NewValue;

            (d as Titlebar).TitlebarBackButton.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
            (d as Titlebar).UpdateTitlebarPadding();
        }

        private static void OnNavigationViewModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Titlebar).UpdateTitlebarPadding();
        }

        private static void OnIsPaneOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Titlebar).UpdateTitlebarPadding();
        }

        private void TitlebarBackButton_Click(object sender, RoutedEventArgs e)
        {
            BackRequested?.Invoke(this, EventArgs.Empty);
        }

        private void UpdateTitlebarPadding()
        {
            if (PaneDisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Compact && !IsBackButtonVisible && !IsPaneOpen)
            {
                var currentPadding = CoreTitlebarPadding;
                currentPadding.Left = 40.0;
                CoreTitlebarPadding = currentPadding;
            }
            else
            {
                var currentPadding = CoreTitlebarPadding;
                currentPadding.Left = 0.0;
                CoreTitlebarPadding = currentPadding;
            }
        }
    }
}
