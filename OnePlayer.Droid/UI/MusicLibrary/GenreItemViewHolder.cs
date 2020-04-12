using Android.Views;
using Android.Widget;

namespace OnePlayer.Droid.UI.MusicLibrary
{
    public class GenreItemViewHolder : Android.Support.V7.Widget.RecyclerView.ViewHolder
    {
        public TextView GenreInitials;
        public TextView GenreName;

        public GenreItemViewHolder(View view) : base(view)
        {
            GenreInitials = view.FindViewById<TextView>(Resource.Id.genre_initials);
            GenreName = view.FindViewById<TextView>(Resource.Id.genre_name);
        }
    }
}