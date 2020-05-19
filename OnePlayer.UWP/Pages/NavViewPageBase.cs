using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace OnePlayer.UWP.Pages
{
    public class NavViewPageBase : Page
    {
        public string HeaderText
        {
            get { return (string)GetValue(HeaderTextProperty); }
            set { SetValue(HeaderTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderTextProperty =
            DependencyProperty.Register("HeaderText", typeof(string), typeof(NavViewPageBase), new PropertyMetadata(""));

        public bool ShowHeader
        {
            get { return (bool)GetValue(ShowHeaderProperty); }
            set { SetValue(ShowHeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowHeader.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowHeaderProperty =
            DependencyProperty.Register("ShowHeader", typeof(bool), typeof(NavViewPageBase), new PropertyMetadata(true));

        public Brush ShellBackground
        {
            get { return (Brush)GetValue(ShellBackgroundProperty); }
            set { SetValue(ShellBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShellBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShellBackgroundProperty =
            DependencyProperty.Register("ShellBackground", typeof(Brush), typeof(NavViewPageBase), new PropertyMetadata(new SolidColorBrush(Colors.Transparent)));


        public double HeaderOpacity
        {
            get { return (double)GetValue(HeaderOpacityProperty); }
            set { SetValue(HeaderOpacityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderOpacity.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderOpacityProperty =
            DependencyProperty.Register("HeaderOpacity", typeof(double), typeof(NavViewPageBase), new PropertyMetadata(1.0));



    }
}
