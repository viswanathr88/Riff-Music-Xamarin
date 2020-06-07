using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Views;
using System.Collections.Generic;

namespace Riff.Droid.UI.MusicLibrary
{
    public class MusicLibraryPagerAdapter : Android.Support.V4.App.FragmentPagerAdapter
    {
        private readonly List<Android.Support.V4.App.Fragment> tabFragments = new List<Android.Support.V4.App.Fragment>();
        private readonly Context context;

        public MusicLibraryPagerAdapter(Android.Support.V4.App.FragmentManager fm, Bundle bundle, Context context)
            : base(fm)
        {
            this.context = context;

            var app = Application.Context as IRiffApp;

            tabFragments.Add(new LibraryArtistsFragment(app.MusicLibrary) { Arguments = bundle });
            tabFragments.Add(new LibraryAlbumsFragment(app.MusicLibrary) { Arguments = bundle });
            tabFragments.Add(new LibraryTracksFragment(app.MusicLibrary) { Arguments = bundle });
            tabFragments.Add(new LibraryGenresFragment(app.MusicLibrary) { Arguments = bundle });
        }

        public override Android.Support.V4.App.Fragment GetItem(int position)
        {
            return tabFragments[position];
        }

        public override Java.Lang.ICharSequence GetPageTitleFormatted(int position)
        {
            var title = position switch
            {
                0 => this.context.GetString(Resource.String.tab_artists),
                1 => this.context.GetString(Resource.String.tab_albums),
                2 => this.context.GetString(Resource.String.tab_tracks),
                3 => this.context.GetString(Resource.String.tab_genres),
                _ => string.Empty,
            };

            return new Java.Lang.String(title);
        }

        public override int Count
        {
            get { return tabFragments.Count; }
        }
    }

    public class MusicLibraryFragment : Android.Support.V4.App.Fragment
    {
        private readonly Context context;

        public MusicLibraryFragment(Context context)
        {
            this.context = context;
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_musiclibrary, container, false);

            ViewPager viewPager = view.FindViewById<ViewPager>(Resource.Id.viewPager);
            viewPager.OffscreenPageLimit = 4;
            var adapter = new MusicLibraryPagerAdapter(ChildFragmentManager, savedInstanceState, this.context);
            viewPager.Adapter = adapter;

            

            //TabLayout 
            TabLayout tabLayout = view.FindViewById<TabLayout>(Resource.Id.sliding_tabs);
            tabLayout.SetupWithViewPager(viewPager);

            return view;
        }
    }
}