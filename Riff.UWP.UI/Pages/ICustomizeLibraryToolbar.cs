using Windows.UI.Xaml.Controls;

namespace Riff.UWP.Pages
{
    public interface ICustomizeLibraryToolbar
    {
        bool ShowSortMenu { get; set; }

        MenuFlyout SortMenu { get; set; }

        bool ShowLayoutMenu { get; set; }

        MenuFlyout LayoutMenu { get; set; }
    }
}
