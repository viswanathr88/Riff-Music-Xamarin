using Android.Support.V7.Widget;
using Android.Views;
using OnePlayer.Data;
using System.Collections.Generic;

namespace OnePlayer.Droid.UI.MusicLibrary
{
    class TrackListAdapter : Android.Support.V7.Widget.RecyclerView.Adapter
    {
        private readonly Data.MusicLibrary library;
        private readonly IList<Track> tracks;

        public TrackListAdapter(Data.MusicLibrary library)
        {
            this.library = library;
            this.tracks = this.library.GetTracks();
        }
        public override int ItemCount => tracks.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var viewHolder = holder as TrackItemViewHolder;
            viewHolder.TrackName.Text = tracks[position].Title;
            viewHolder.TrackArtist.Text = tracks[position].Artist;
            viewHolder.TrackDuration.Text = tracks[position].Duration.ToString();

            if (this.library.TrackArts.Exists(tracks[position].Id, ThumbnailSize.Small))
            {
                using var stream = this.library.TrackArts.Get(tracks[position].Id, ThumbnailSize.Small);
                viewHolder.TrackArt.SetImageBitmap(Android.Graphics.BitmapFactory.DecodeStream(stream));
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // Create a new view for the album item
            var view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.widget_track_item, parent, false);
            return new TrackItemViewHolder(view);
        }
    }
}