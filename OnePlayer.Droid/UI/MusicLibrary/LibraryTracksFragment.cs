using Android.Views;

namespace OnePlayer.Droid.UI.MusicLibrary
{
    public class LibraryTracksFragment : Controls.LazyLoadedFragment
    {
        public LibraryTracksFragment() : base(Resource.Layout.fragment_musiclibrary_tracks)
        {
        }

        protected override void Load()
        {
            var recyclerView = (Android.Support.V7.Widget.RecyclerView)View.FindViewById(Resource.Id.tracks_recycler_view);

            // use this setting to improve performance if you know that changes
            // in content do not change the layout size of the RecyclerView
            recyclerView.HasFixedSize = true;

            // use a linear layout manager
            var layoutManager = new Android.Support.V7.Widget.LinearLayoutManager(Activity);
            recyclerView.SetLayoutManager(layoutManager);
            recyclerView.AddItemDecoration(new Controls.VerticalSpaceItemDecoration(25));

            // specify an adapter (see also next example)
            var adapter = new TrackListAdapter();
            recyclerView.SetAdapter(adapter);
        }
    }
}