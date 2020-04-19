using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace OnePlayer.UWP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MusicLibraryPage : Page
    {
        public MusicLibraryPage()
        {
            this.InitializeComponent();
            Loaded += MusicLibraryPage_Loaded;
        }

        private void MusicLibraryPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Simulate the first entry in the filter being selected
            RadioMenuFlyoutItem_Click(ToolbarFilterFlyout.Items.First(), new RoutedEventArgs());

            // Simulate the first entry in the sort being selected
            SortFlyoutItem_Click(ToolbarSortFlyout.Items.First(), new RoutedEventArgs());

            // Simulate the first entry in display mode being selected;
            DisplayModeFlyoutItem_Click(ToolbarDisplayModeFlyout.Items.First(), new RoutedEventArgs());
        }

        private void RadioMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as Microsoft.UI.Xaml.Controls.RadioMenuFlyoutItem;
            item.IsChecked = true;
            if (item != null)
            {
                var fontIcon = item.Icon as FontIcon;
                ToolbarFilter.Icon = new FontIcon() { Glyph = fontIcon.Glyph };
                ToolbarFilter.Label = item.Text;
                ToolbarText.Text = item.Text;
            }
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SortFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Microsoft.UI.Xaml.Controls.RadioMenuFlyoutItem);
            if (item != null)
            {
                item.IsChecked = true;
                ToolbarSort.Label = item.Text;
            }
        }

        private void SortOrderFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
        }

        private void DisplayModeFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as Microsoft.UI.Xaml.Controls.RadioMenuFlyoutItem);
            if (item != null)
            {
                item.IsChecked = true;
                var fontIcon = item.Icon as FontIcon;
                ToolbarDisplayMode.Icon = new FontIcon() { Glyph = fontIcon.Glyph };
                ToolbarDisplayMode.Label = item.Text;
            }
        }
    }
}
