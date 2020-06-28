using Riff.Data;

namespace Riff.Droid.UI.MusicLibrary
{
    public class LibraryTracksFragment : Controls.LazyLoadedFragment
    {
        private readonly IMusicLibrary library;
        public LibraryTracksFragment(IMusicLibrary library) : base(Resource.Layout.fragment_musiclibrary_tracks)
        {
            this.library = library;
        }

        protected override void Load(Android.Views.View view)
        {
            var recyclerView = (Android.Support.V7.Widget.RecyclerView)view.FindViewById(Resource.Id.tracks_recycler_view);

            // use this setting to improve performance if you know that changes
            // in content do not change the layout size of the RecyclerView
            recyclerView.HasFixedSize = true;

            // use a linear layout manager
            var layoutManager = new Android.Support.V7.Widget.LinearLayoutManager(Activity);
            recyclerView.SetLayoutManager(layoutManager);
            recyclerView.AddItemDecoration(new Controls.VerticalSpaceItemDecoration(25));

            // specify an adapter (see also next example)
            var adapter = new TrackListAdapter(this.library, OnTrackSelected);
            recyclerView.SetAdapter(adapter);
        }

        private void OnTrackSelected(Track track)
        {

        }
    }
}