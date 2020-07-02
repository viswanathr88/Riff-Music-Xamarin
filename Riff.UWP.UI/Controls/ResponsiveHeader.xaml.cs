using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Riff.UWP.Controls
{
    public sealed partial class ResponsiveHeader : UserControl
    {
        public ResponsiveHeader()
        {
            this.InitializeComponent();
        }

        public UIElement Image
        {
            get { return (UIElement)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Image.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(UIElement), typeof(ResponsiveHeader), new PropertyMetadata(null));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(ResponsiveHeader), new PropertyMetadata(string.Empty));

        public object Subheader
        {
            get { return (object)GetValue(SubheaderProperty); }
            set { SetValue(SubheaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Subheader.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SubheaderProperty =
            DependencyProperty.Register("Subheader", typeof(object), typeof(ResponsiveHeader), new PropertyMetadata(null, OnSubheaderChanged));

        private static void OnSubheaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ResponsiveHeader).UpdateSubheaderVisibility();
        }

        private void UpdateSubheaderVisibility()
        {
            SubheaderControl.Visibility = (Subheader != null) ? Visibility.Visible : Visibility.Collapsed;
        }

        public CommandBar Toolbar
        {
            get { return (CommandBar)GetValue(ToolbarProperty); }
            set { SetValue(ToolbarProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Toolbar.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ToolbarProperty =
            DependencyProperty.Register("Toolbar", typeof(CommandBar), typeof(ResponsiveHeader), new PropertyMetadata(null));

    }
}
