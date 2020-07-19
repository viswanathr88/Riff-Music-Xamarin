using Mirage.ViewModel.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
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

namespace Riff.UWP.Controls
{
    public sealed partial class OptionsOverlayControl : UserControl
    {
        public OptionsOverlayControl()
        {
            this.InitializeComponent();
        }

        #region Play Command

        public ICommand PlayCommand
        {
            get { return (ICommand)GetValue(PlayCommandProperty); }
            set { SetValue(PlayCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PlayCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlayCommandProperty =
            DependencyProperty.Register("PlayCommand", typeof(ICommand), typeof(OptionsOverlayControl), new PropertyMetadata(null));

        public object PlayCommandParameter
        {
            get { return (object)GetValue(PlayCommandParameterProperty); }
            set { SetValue(PlayCommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PlayCommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlayCommandParameterProperty =
            DependencyProperty.Register("PlayCommandParameter", typeof(object), typeof(OptionsOverlayControl), new PropertyMetadata(null));

        #endregion

        #region Play Next Command
        public ICommand PlayNextCommand
        {
            get { return (ICommand)GetValue(PlayNextCommandProperty); }
            set { SetValue(PlayNextCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PlayNextCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlayNextCommandProperty =
            DependencyProperty.Register("PlayNextCommand", typeof(ICommand), typeof(OptionsOverlayControl), new PropertyMetadata(null));

        public object PlayNextCommandParameter
        {
            get { return (object)GetValue(PlayNextCommandParameterProperty); }
            set { SetValue(PlayNextCommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PlayNextCommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlayNextCommandParameterProperty =
            DependencyProperty.Register("PlayNextCommandParameter", typeof(object), typeof(OptionsOverlayControl), new PropertyMetadata(null));


        #endregion

        #region Add to Playlist Command

        public ICommand AddToPlaylistCommand
        {
            get { return (ICommand)GetValue(AddToPlaylistCommandProperty); }
            set { SetValue(AddToPlaylistCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AddToPlaylistCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddToPlaylistCommandProperty =
            DependencyProperty.Register("AddToPlaylistCommand", typeof(ICommand), typeof(OptionsOverlayControl), new PropertyMetadata(null));



        public object AddToPlaylistCommandParameter
        {
            get { return (object)GetValue(AddToPlaylistCommandParameterProperty); }
            set { SetValue(AddToPlaylistCommandParameterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AddToPlaylistCommandParameter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddToPlaylistCommandParameterProperty =
            DependencyProperty.Register("AddToPlaylistCommandParameter", typeof(object), typeof(OptionsOverlayControl), new PropertyMetadata(null));


        public bool AllowAddingToPlaylist
        {
            get { return (bool)GetValue(AllowAddingToPlaylistProperty); }
            set { SetValue(AllowAddingToPlaylistProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AllowAddingToPlaylist.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AllowAddingToPlaylistProperty =
            DependencyProperty.Register("AllowAddingToPlaylist", typeof(bool), typeof(OptionsOverlayControl), new PropertyMetadata(true));

        #endregion

    }
}
