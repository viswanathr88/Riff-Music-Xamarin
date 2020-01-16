using Android.Views;
using Android.Widget;

namespace OnePlayer.Droid.UI.MusicLibrary
{
    public class TrackItemViewHolder : Android.Support.V7.Widget.RecyclerView.ViewHolder
    {
        public ImageView TrackArt;
        public TextView TrackName;
        public TextView TrackArtist;
        public TextView TrackDuration;

        public TrackItemViewHolder(View view) : base(view)
        {
            TrackArt = view.FindViewById<ImageView>(Resource.Id.track_art);
            TrackName = view.FindViewById<TextView>(Resource.Id.track_name);
            TrackArtist = view.FindViewById<TextView>(Resource.Id.track_artist);
            TrackDuration = view.FindViewById<TextView>(Resource.Id.track_duration);
        }
    }
}