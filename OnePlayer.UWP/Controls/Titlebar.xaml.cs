using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace OnePlayer.UWP.Controls
{
    public sealed partial class Titlebar : UserControl
    {
        CoreApplicationViewTitleBar coreTitlebar = CoreApplication.GetCurrentView().TitleBar;

        public Titlebar()
        {
            this.InitializeComponent();
            Loaded += Titlebar_Loaded;
            Unloaded += Titlebar_Unloaded;
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

        private void Titlebar_Loaded(object sender, RoutedEventArgs e)
        {
            coreTitlebar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;

            Window.Current.SetTitleBar(TitlebarMiddleGrid);

            UpdateLayoutMetrics();
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
            this.CoreTitlebarHeight = coreTitlebar.Height;

            // The SystemOverlayLeftInset and SystemOverlayRightInset values are
            // in terms of physical left and right. Therefore, we need to flip
            // then when our flow direction is RTL.
            if (FlowDirection == FlowDirection.LeftToRight)
            {
                this.CoreTitlebarPadding = new Thickness()
                {
                    Left = coreTitlebar.SystemOverlayLeftInset,
                    Right = coreTitlebar.SystemOverlayRightInset
                };
            }
            else
            {
                this.CoreTitlebarPadding = new Thickness()
                {
                    Left = coreTitlebar.SystemOverlayRightInset,
                    Right = coreTitlebar.SystemOverlayLeftInset
                };
            }
        }

        private static void OnIsBackButtonVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            bool isVisible = (bool)e.NewValue;

            (d as Titlebar).TitlebarBackButton.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void TitlebarBackButton_Click(object sender, RoutedEventArgs e)
        {
            BackRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
