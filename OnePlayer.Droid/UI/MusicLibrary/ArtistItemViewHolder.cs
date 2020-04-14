using Android.Views;
using Android.Widget;
using System;

namespace OnePlayer.Droid.UI.MusicLibrary
{
    public class ArtistItemViewHolder : Android.Support.V7.Widget.RecyclerView.ViewHolder
    {
        public TextView ArtistInitials;
        public TextView ArtistName;
        private readonly Action<int> listener;

        public ArtistItemViewHolder(View view, Action<int> listener) : base(view)
        {
            ArtistInitials = view.FindViewById<TextView>(Resource.Id.artist_initials);
            ArtistName = view.FindViewById<TextView>(Resource.Id.artist_name);
            this.listener = listener;

            view.Click += (object sender, System.EventArgs e) => listener(LayoutPosition);
        }
    }
}