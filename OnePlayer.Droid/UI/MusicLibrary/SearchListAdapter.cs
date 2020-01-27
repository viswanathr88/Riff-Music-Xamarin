using Android.App;
using Android.Views;
using Android.Widget;
using OnePlayer.Data;
using System.Collections.Generic;

namespace OnePlayer.Droid.UI.MusicLibrary
{
    public class SearchListAdapter : BaseAdapter<SearchItem>
    {
        private readonly IList<SearchItem> searchItems;
        private readonly int itemLayoutId;
        private readonly Activity activity;

        public SearchListAdapter(Activity activity, int itemLayoutId, IList<SearchItem> items)
        {
            this.activity = activity;
            this.itemLayoutId = itemLayoutId;
            searchItems = items;
        }

        public override SearchItem this[int position] => searchItems[position];
         
        public override int Count => searchItems.Count;

        public override long GetItemId(int position)
        {
            return 0;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = LayoutInflater.From(activity).Inflate(itemLayoutId, null);
            var name = view.FindViewById<TextView>(Resource.Id.search_item_name);
            name.Text = searchItems[position].Name;

            var description = view.FindViewById<TextView>(Resource.Id.search_item_description);
            
            if (searchItems[position].Type == SearchItemType.Album)
            {
                description.Text = activity.Resources.GetString(Resource.String.search_album_description, searchItems[position].Description);
            }
            else if (searchItems[position].Type == SearchItemType.Artist)
            {
                description.Text = activity.Resources.GetString(Resource.String.search_artist_description);
            }
            else if (searchItems[position].Type == SearchItemType.Genre)
            {
                description.Text = activity.Resources.GetString(Resource.String.search_genre_description);
            }
            else if (searchItems[position].Type == SearchItemType.Track || searchItems[position].Type == SearchItemType.TrackArtist)
            {
                description.Text = activity.Resources.GetString(Resource.String.search_track_description, searchItems[position].Description);
            }

            return view;
        }
    }
}