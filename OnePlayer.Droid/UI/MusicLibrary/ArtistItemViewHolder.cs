using Android.Views;
using Android.Widget;

namespace OnePlayer.Droid.UI.MusicLibrary
{
    public class ArtistItemViewHolder : Android.Support.V7.Widget.RecyclerView.ViewHolder
    {
        public TextView ArtistInitials;
        public TextView ArtistName;

        public ArtistItemViewHolder(View view) : base(view)
        {
            ArtistInitials = view.FindViewById<TextView>(Resource.Id.artist_initials);
            ArtistName = view.FindViewById<TextView>(Resource.Id.artist_name);
        }
    }
}