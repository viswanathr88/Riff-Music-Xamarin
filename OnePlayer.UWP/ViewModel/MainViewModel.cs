﻿using OnePlayer.Data;
using OnePlayer.Sync;
using System;
using System.Threading.Tasks;

namespace OnePlayer.UWP.ViewModel
{
    public sealed class MainViewModel : DataViewModel
    {
        private readonly MusicLibrary library;
        private readonly SyncEngine syncEngine;
        private SyncState state;

        private readonly Lazy<SearchSuggestionsViewModel> searchSuggestionsVM;

        public MainViewModel(MusicLibrary library, SyncEngine engine)
        {
            this.library = library ?? throw new ArgumentNullException(nameof(library));
            this.syncEngine = engine ?? throw new ArgumentNullException(nameof(engine));
            this.syncEngine.StateChanged += SyncEngine_StateChanged;
            State = this.syncEngine.State;

            this.searchSuggestionsVM = new Lazy<SearchSuggestionsViewModel>(() => new SearchSuggestionsViewModel(library));
        }

        public SyncState State
        {
            get
            {
                return this.state;
            }
            private set
            {
                SetProperty(ref this.state, value);
            }
        }

        public SearchSuggestionsViewModel SearchSuggestions => searchSuggestionsVM.Value;

        public override Task LoadAsync()
        {
            Task.Run(async() => await syncEngine.RunAsync());
            return Task.CompletedTask;
        }

        public void Load()
        {
            Task.Run(async () => await syncEngine.RunAsync());
        }

        private void SyncEngine_StateChanged(object sender, SyncState state)
        {
            State = state;
        }
    }
}
