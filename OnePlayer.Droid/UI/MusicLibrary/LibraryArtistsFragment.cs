using Android.Support.V7.Widget;
using OnePlayer.Data;

namespace OnePlayer.Droid.UI.MusicLibrary
{
    public class LibraryArtistsFragment : Controls.LazyLoadedFragment
    {
        private readonly Data.MusicLibrary library;
        public LibraryArtistsFragment(Data.MusicLibrary library) : base(Resource.Layout.fragment_musiclibrary_artists)
        {
            this.library = library;
        }

        protected override void Load(Android.Views.View view)
        {
            var recyclerView = (RecyclerView)view.FindViewById(Resource.Id.artists_recycler_view);

            // use this setting to improve performance if you know that changes
            // in content do not change the layout size of the RecyclerView
            recyclerView.HasFixedSize = true;

            // use a linear layout manager
            var layoutManager = new Android.Support.V7.Widget.LinearLayoutManager(Activity);
            recyclerView.SetLayoutManager(layoutManager);
            //recyclerView.AddItemDecoration(new Controls.VerticalSpaceItemDecoration(25));

            // specify an adapter (see also next example)
            var adapter = new ArtistListAdapter(this.library, OnArtistSelected);
            recyclerView.SetAdapter(adapter);
        }

        private void OnArtistSelected(Artist artist)
        {
            // TODO: Launch Artist activity
        }
    }
}