using Riff.UWP.ViewModel;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Riff.UWP.Pages
{
    /// <summary>
    /// Represents any page that is rendered within the NavigationView
    /// </summary>
    public abstract class NavViewPageBase : PageBase
    {
        public string HeaderText
        {
            get { return (string)GetValue(HeaderTextProperty); }
            set { SetValue(HeaderTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderTextProperty =
            DependencyProperty.Register("HeaderText", typeof(string), typeof(NavViewPageBase), new PropertyMetadata(""));

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
