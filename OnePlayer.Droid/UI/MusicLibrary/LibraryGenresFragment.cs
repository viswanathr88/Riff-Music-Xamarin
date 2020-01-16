using Android.OS;
using Android.Views;

namespace OnePlayer.Droid.UI.MusicLibrary
{
    public class LibraryGenresFragment : Android.Support.V4.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            return inflater.Inflate(Resource.Layout.fragment_musiclibrary_genres, container, false);
        }
    }
}