using Android.Support.V7.Widget;
using Android.Views;

namespace OnePlayer.Droid.UI.MusicLibrary
{
    public partial class AlbumListAdapter : Android.Support.V7.Widget.RecyclerView.Adapter
    {
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            // Create a new view for the album item
            var view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.widget_album_item, parent, false);
            return new AlbumItemViewHolder(view);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var albumItemViewHolder = holder as AlbumItemViewHolder;
            albumItemViewHolder.AlbumName.Text = $"Album Name {position + 1}";
            albumItemViewHolder.ArtistName.Text = $"Artist Name {position + 1}";
        }

        public override int ItemCount => 500;
    }
}