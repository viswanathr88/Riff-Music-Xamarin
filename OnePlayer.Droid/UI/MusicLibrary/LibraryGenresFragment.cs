using Android.Support.V7.Widget;
using OnePlayer.Data;

namespace OnePlayer.Droid.UI.MusicLibrary
{
    public class LibraryGenresFragment : Controls.LazyLoadedFragment
    {
        private readonly Data.MusicLibrary library;
        public LibraryGenresFragment(Data.MusicLibrary library) : base(Resource.Layout.fragment_musiclibrary_genres)
        {
            this.library = library;
        }

        protected override void Load(Android.Views.View view)
        {
            var recyclerView = (RecyclerView)view.FindViewById(Resource.Id.genres_recycler_view);

            // use this setting to improve performance if you know that changes
            // in content do not change the layout size of the RecyclerView
            recyclerView.HasFixedSize = true;

            // use a linear layout manager
            var layoutManager = new Android.Support.V7.Widget.LinearLayoutManager(Activity);
            recyclerView.SetLayoutManager(layoutManager);

            // specify an adapter (see also next example)
            var adapter = new GenreListAdapter(this.library, OnGenreSelected);
            recyclerView.SetAdapter(adapter);
        }

        private void OnGenreSelected(Genre genre)
        {

        }
    }
}