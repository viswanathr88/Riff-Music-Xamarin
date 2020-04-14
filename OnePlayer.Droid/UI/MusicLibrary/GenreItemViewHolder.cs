using Android.Views;
using Android.Widget;
using System;

namespace OnePlayer.Droid.UI.MusicLibrary
{
    public class GenreItemViewHolder : Android.Support.V7.Widget.RecyclerView.ViewHolder
    {
        public TextView GenreInitials;
        public TextView GenreName;
        private readonly Action<int> listener;

        public GenreItemViewHolder(View view, Action<int> listener) : base(view)
        {
            GenreInitials = view.FindViewById<TextView>(Resource.Id.genre_initials);
            GenreName = view.FindViewById<TextView>(Resource.Id.genre_name);
            this.listener = listener;

            view.Click += (object sender, System.EventArgs e) => listener(LayoutPosition);
        }
    }
}