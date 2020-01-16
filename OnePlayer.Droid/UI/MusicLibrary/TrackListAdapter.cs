using Android.Support.V7.Widget;
using Android.Views;

namespace OnePlayer.Droid.UI.MusicLibrary
{
    class TrackListAdapter : Android.Support.V7.Widget.RecyclerView.Adapter
    {
        public override int ItemCount => 100;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var viewHolder = holder as TrackItemViewHolder;
            viewHolder.TrackName.Text = $"Untitled Track {position + 1}";
            viewHolder.TrackArtist.Text = $"Artist Name {position + 1}";
            viewHolder.TrackDuration.Text = "04:00";
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // Create a new view for the album item
            var view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.widget_track_item, parent, false);
            return new TrackItemViewHolder(view);
        }
    }
}