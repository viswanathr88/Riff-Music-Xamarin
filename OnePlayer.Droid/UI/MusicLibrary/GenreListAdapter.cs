using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using OnePlayer.Data;
using System;
using System.Collections.Generic;

namespace OnePlayer.Droid.UI.MusicLibrary
{
    class GenreListAdapter : Android.Support.V7.Widget.RecyclerView.Adapter
    {
        private readonly Data.MusicLibrary library;
        private readonly IList<Genre> genres = new List<Genre>();
        private readonly Random rnd = new Random(Guid.NewGuid().GetHashCode());
        private static readonly string[] separators = new string[] { " ", ".", "&", "-" };

        public GenreListAdapter(Data.MusicLibrary library)
        {
            this.library = library;
            this.genres = this.library.GetGenres();
        }
        public override int ItemCount => genres.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var viewHolder = holder as GenreItemViewHolder;
            viewHolder.GenreName.Text = this.genres[position].Name;
            viewHolder.GenreInitials.Text = GetInitials(genres[position].Name).ToUpper();
            Color randomColor = Color.Argb(255, rnd.Next(256), rnd.Next(256), rnd.Next(256));
            viewHolder.GenreInitials.SetBackgroundColor(randomColor);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // Create a new view for the album item
            var view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.widget_genre_item, parent, false);
            return new GenreItemViewHolder(view);
        }

        private string GetInitials(string name)
        {
            var tokens = name.Split(separators, System.StringSplitOptions.RemoveEmptyEntries);
            string initials;
            if (tokens.Length == 1)
            {
                initials = tokens[0].Substring(0, Math.Min(tokens[0].Length, 2));
            }
            else
            {
                initials = new string(new char[] { tokens[0][0], tokens[1][0] });
            }

            return initials;
        }
    }
}