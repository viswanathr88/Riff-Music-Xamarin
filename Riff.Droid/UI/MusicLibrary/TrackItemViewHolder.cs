using Android.Views;
using Android.Widget;
using System;

namespace Riff.Droid.UI.MusicLibrary
{
    public class TrackItemViewHolder : Android.Support.V7.Widget.RecyclerView.ViewHolder
    {
        public ImageView TrackArt;
        public TextView TrackName;
        public TextView TrackArtist;
        public TextView TrackDuration;
        private readonly Action<int> listener;

        public TrackItemViewHolder(View view, Action<int> listener) : base(view)
        {
            TrackArt = view.FindViewById<ImageView>(Resource.Id.track_art);
            TrackName = view.FindViewById<TextView>(Resource.Id.track_name);
            TrackArtist = view.FindViewById<TextView>(Resource.Id.track_artist);
            TrackDuration = view.FindViewById<TextView>(Resource.Id.track_duration);
            this.listener = listener;

            view.Click += (object sender, EventArgs e) => listener(LayoutPosition);
        }
    }
}