using Android.Views;
using Android.Widget;

namespace Riff.Droid.UI.MusicLibrary
{
    public class AlbumItemViewHolder : Android.Support.V7.Widget.RecyclerView.ViewHolder
    {
        public ImageView AlbumArt;
        public TextView AlbumName;
        public TextView ArtistName;
        public AlbumItemViewHolder(View view) : base(view)
        {
            AlbumArt = view.FindViewById<ImageView>(Resource.Id.album_art);
            AlbumName = view.FindViewById<TextView>(Resource.Id.album_name);
            ArtistName = view.FindViewById<TextView>(Resource.Id.album_artist);
        }
    }
}