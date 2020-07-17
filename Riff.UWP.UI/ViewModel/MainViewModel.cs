using Mirage.ViewModel;
using Riff.Data;
using Riff.Sync;
using System;
using System.Threading.Tasks;

namespace Riff.UWP.ViewModel
{
    public sealed class MainViewModel : DataViewModel
    {
        private readonly IMusicLibrary library;

        private readonly Lazy<SearchSuggestionsViewModel> searchSuggestionsVM;
        private readonly Lazy<SyncViewModel> syncVM;

        public MainViewModel(IMusicLibrary library, SyncEngine engine, IPreferences preferences)
        {
            this.library = library ?? throw new ArgumentNullException(nameof(library));

            this.searchSuggestionsVM = new Lazy<SearchSuggestionsViewModel>(() => new SearchSuggestionsViewModel(library));
            this.syncVM = new Lazy<SyncViewModel>(() => new SyncViewModel(engine, preferences));
        }

        public SearchSuggestionsViewModel SearchSuggestions => searchSuggestionsVM.Value;

        public SyncViewModel Sync => syncVM.Value;

        public override async Task LoadAsync()
        {
            await Sync.LoadAsync();
        }

        public void Load()
        {
            Sync.Load();
        }
    }
}
