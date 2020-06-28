using Android.Support.V7.Widget;
using Riff.Data;

namespace Riff.Droid.UI.MusicLibrary
{
    public class LibraryAlbumsFragment : Controls.LazyLoadedFragment
    {
        private readonly IMusicLibrary library;
        public LibraryAlbumsFragment(IMusicLibrary library) : base(Resource.Layout.fragment_musiclibrary_albums)
        {
            this.library = library;
        }

        protected override void Load(Android.Views.View view)
        {
            var recyclerView = (RecyclerView)view.FindViewById(Resource.Id.albums_recycler_view);

            // use this setting to improve performance if you know that changes
            // in content do not change the layout size of the RecyclerView
            recyclerView.HasFixedSize = true;

            // use a linear layout manager
            var layoutManager = new GridLayoutManager(Activity, 3);
            recyclerView.SetLayoutManager(layoutManager);
            recyclerView.AddItemDecoration(new Controls.ItemOffsetDecoration(20));

            // specify an adapter (see also next example)
            var adapter = new AlbumListAdapter(library);
            recyclerView.SetAdapter(adapter);
        }
    }
}