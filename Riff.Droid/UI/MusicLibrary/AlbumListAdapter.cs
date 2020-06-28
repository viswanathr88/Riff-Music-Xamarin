using Android.Support.V7.Widget;
using Android.Views;
using Riff.Data;
using System.Collections.Generic;

namespace Riff.Droid.UI.MusicLibrary
{
    public partial class AlbumListAdapter : Android.Support.V7.Widget.RecyclerView.Adapter
    {
        private readonly IMusicLibrary library;
        private readonly IList<Album> albums;

        public AlbumListAdapter(IMusicLibrary library)
        {
            this.library = library;
            this.albums = library.Albums.Get();
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // Create a new view for the album item
            var view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.widget_album_item, parent, false);
            return new AlbumItemViewHolder(view);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var albumItemViewHolder = holder as AlbumItemViewHolder;
            albumItemViewHolder.AlbumName.Text = albums[position].Name;
            albumItemViewHolder.ArtistName.Text = albums[position].Artist.Name;
            if (library.AlbumArts.Exists(albums[position].Id.Value))
            {
                using var stream = library.AlbumArts.Get(albums[position].Id.Value);
                albumItemViewHolder.AlbumArt.SetImageBitmap(Android.Graphics.BitmapFactory.DecodeStream(stream));
            }
            else
            {
                albumItemViewHolder.AlbumArt.SetImageResource(Resource.Drawable.ic_menu_album);
                // albumItemViewHolder.AlbumArt.SetImageDrawable(null);
            }
        }

        public override int ItemCount => albums.Count;
    }
}