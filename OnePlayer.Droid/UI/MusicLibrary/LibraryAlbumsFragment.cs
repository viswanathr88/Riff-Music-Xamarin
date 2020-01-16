using Android.Support.V7.Widget;
using Android.Views;

namespace OnePlayer.Droid.UI.MusicLibrary
{
    public class LibraryAlbumsFragment : Controls.LazyLoadedFragment
    {
        public LibraryAlbumsFragment() : base(Resource.Layout.fragment_musiclibrary_albums)
        {
        }

        protected override void Load()
        {
            var recyclerView = (RecyclerView)View.FindViewById(Resource.Id.albums_recycler_view);

            // use this setting to improve performance if you know that changes
            // in content do not change the layout size of the RecyclerView
            recyclerView.HasFixedSize = true;

            // use a linear layout manager
            var layoutManager = new GridLayoutManager(Activity, 3);
            recyclerView.SetLayoutManager(layoutManager);
            recyclerView.AddItemDecoration(new Controls.ItemOffsetDecoration(20));

            // specify an adapter (see also next example)
            var adapter = new AlbumListAdapter();
            recyclerView.SetAdapter(adapter);
        }
    }
}