using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Riff.Data;
using System.Collections.Generic;

namespace Riff.Droid.UI.MusicLibrary
{
    [Activity]
    public class SearchActivity: Android.Support.V7.App.AppCompatActivity
    {
        private readonly List<SearchItem> searchItems = new List<SearchItem>();
        private SearchListAdapter adapter = null;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_search);

            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            ListView view = FindViewById<ListView>(Resource.Id.search_result_list);
            adapter = new SearchListAdapter(this, Resource.Layout.widget_search_item, this.searchItems);
            view.Adapter = adapter;
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_search_activity, menu);
            var searchView = (Android.Support.V7.Widget.SearchView)menu.FindItem(Resource.Id.search).ActionView;
            searchView.Iconified = false;
            searchView.QueryHint = "Search your music collection";
            searchView.QueryTextChange += SearchView_QueryTextChange;
            searchView.QueryTextSubmit += SearchView_QueryTextSubmit;
            searchView.Close += SearchView_Close;
            return base.OnCreateOptionsMenu(menu);
        }

        private void SearchView_Close(object sender, Android.Support.V7.Widget.SearchView.CloseEventArgs e)
        {
            Finish();
        }

        private void SearchView_QueryTextSubmit(object sender, Android.Support.V7.Widget.SearchView.QueryTextSubmitEventArgs e)
        {
            
        }

        private void SearchView_QueryTextChange(object sender, Android.Support.V7.Widget.SearchView.QueryTextChangeEventArgs e)
        {
            var app = Application as IRiffApp;
            searchItems.Clear();

            if (!string.IsNullOrEmpty(e.NewText))
            {
                app.MusicLibrary.Search(new SearchQuery() { Term = e.NewText }, searchItems);
            }
            
            adapter.NotifyDataSetChanged();

        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    break;
                default:
                    break;
            }

            return base.OnOptionsItemSelected(item);
        }
    }
}