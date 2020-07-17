using Mirage.ViewModel;
using Riff.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Riff.UWP.ViewModel
{
    public sealed class SearchSuggestionsViewModel : ViewModelBase
    {
        private readonly IMusicLibrary library;
        private ObservableCollection<SearchItemViewModel> items;

        public SearchSuggestionsViewModel(IMusicLibrary library)
        {
            this.library = library ?? throw new ArgumentNullException(nameof(library));
        }

        public ObservableCollection<SearchItemViewModel> Items
        {
            get
            {
                return this.items;
            }
            private set
            {
                SetProperty(ref this.items, value);
            }
        }

        public async Task Load(string searchTerm)
        {
            var query = new SearchQuery()
            {
                Term = searchTerm
            };

            IList<SearchItem> results = null;
            await Task.Run(() => results = this.library.Search(query));

            Items = new ObservableCollection<SearchItemViewModel>();
            foreach (var item in results)
            {
                Items.Add(await CreateItemViewModel(item));
            }
        }

        private async Task<SearchItemViewModel> CreateItemViewModel(SearchItem item)
        {
            var itemVM = new SearchItemViewModel(library, item);
            await itemVM.LoadArtAsync();
            return itemVM;
        }
    }
}
