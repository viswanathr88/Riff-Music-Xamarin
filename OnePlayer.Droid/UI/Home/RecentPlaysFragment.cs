using Android.OS;
using Android.Views;

namespace OnePlayer.Droid.UI.Home
{
    public class RecentPlaysFragment : Android.Support.V4.App.Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.fragment_recentplays, container, false);
        }
    }
}