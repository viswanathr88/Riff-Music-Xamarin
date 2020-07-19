using Mirage.ViewModel.Commands;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;

namespace Riff.UWP.Pages
{
    public static class MenuFlyoutExtensions
    {
        public static ListViewBase GetAssociatedList(DependencyObject obj)
        {
            return (ListViewBase)obj.GetValue(AssociatedListProperty);
        }

        public static void SetAssociatedList(DependencyObject obj, ListViewBase value)
        {
            obj.SetValue(AssociatedListProperty, value);
        }

        // Using a DependencyProperty as the backing store for AssociatedList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AssociatedListProperty =
            DependencyProperty.RegisterAttached("AssociatedList", typeof(ListViewBase), typeof(MenuFlyoutExtensions), new PropertyMetadata(null, OnPropertyChanged));

        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var menuFlyout = (MenuFlyout)d;
            var listView = (ListViewBase)e.NewValue;

            void OnSelectedItemChanged(DependencyObject sender, DependencyProperty dp)
            {
                menuFlyout.Hide();
                UpdateMenuFlyoutCommandParameter(menuFlyout, listView.SelectedItem);
            };

            menuFlyout.Opening += OnMenuFlyoutOpening;
            menuFlyout.Closing += OnMenuFlyoutClosing;

            listView.RegisterPropertyChangedCallback(Selector.SelectedItemProperty, OnSelectedItemChanged);
        }


        private static void OnMenuFlyoutClosing(FlyoutBase sender, FlyoutBaseClosingEventArgs args)
        {
            var menuFlyout = (MenuFlyout)sender;
            var attachedListView = GetAssociatedList(menuFlyout);
            UpdateMenuFlyoutCommandParameter(menuFlyout, attachedListView.SelectedItem);
        }

        private static void OnMenuFlyoutOpening(object sender, object e)
        {
            var menuFlyout = (MenuFlyout)sender;
            object currentItem = null;
            if (menuFlyout.Target is SelectorItem selectorItem)
            {
                currentItem = selectorItem.Content;
            }
            else
            {
                currentItem = menuFlyout.Target.DataContext;
            }
            UpdateMenuFlyoutCommandParameter(menuFlyout, currentItem);
        }

        public static void UpdateMenuFlyoutCommandParameter(MenuFlyout menuFlyout, object commandParameter)
        {
            foreach (var item in menuFlyout.Items)
            {
                if (item is MenuFlyoutItem menuFlyoutItem)
                {
                    menuFlyoutItem.CommandParameter = commandParameter;
                }
                else if (item is MenuFlyoutSubItem subMenu)
                {
                    foreach (var subitem in subMenu.Items)
                    {
                        if (subitem is MenuFlyoutItem subFlyoutItem)
                        {
                            subFlyoutItem.CommandParameter = commandParameter;
                        }
                    }
                }
            }
        }
    }
}
