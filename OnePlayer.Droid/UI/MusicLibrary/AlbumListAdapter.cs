using Android.Support.V7.Widget;
using Android.Views;
using OnePlayer.Data;
using System.Collections.Generic;

namespace OnePlayer.Droid.UI.MusicLibrary
{
    public partial class AlbumListAdapter : Android.Support.V7.Widget.RecyclerView.Adapter
    {
        private readonly Data.MusicLibrary library;
        private readonly IList<Album> albums;

        public AlbumListAdapter(Data.MusicLibrary library)
        {
            this.library = library;
            this.albums = library.GetAlbums();
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
            albumItemViewHolder.ArtistName.Text = albums[position].ArtistId.ToString();
            if (library.AlbumArts.Exists(albums[position].Id, ThumbnailSize.Medium))
            {
                using var stream = library.AlbumArts.Get(albums[position].Id, ThumbnailSize.Medium);
                albumItemViewHolder.AlbumArt.SetImageBitmap(Android.Graphics.BitmapFactory.DecodeStream(stream));
            }
        }

        public override int ItemCount => albums.Count;
    }
}