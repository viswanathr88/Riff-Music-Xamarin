﻿using Mirage.ViewModel;
using Mirage.ViewModel.Commands;
using Riff.Data;

namespace Riff.UWP.ViewModel
{
    public sealed class AlbumItemViewModel : ItemViewModel<Album>
    {
        private readonly IMusicLibrary library;
        private readonly IAlbumCommands commands;
        private bool isPointerOver = false;

        public AlbumItemViewModel(Album item, IMusicLibrary library, IAlbumCommands commands) : base(item)
        {
            this.commands = commands;
            this.library = library;
        }

        public bool ShowOverlayControls
        {
            get => IsPointerOver || IsSelected;
        }

        public bool IsPointerOver
        {
            get => isPointerOver;
            set
            {
                if (SetProperty(ref this.isPointerOver, value))
                {
                    RaisePropertyChanged(nameof(ShowOverlayControls));
                }
            }
        }

        protected override void OnSelectionChanged()
        {
            base.OnSelectionChanged();

            RaisePropertyChanged(nameof(ShowOverlayControls));
        }

        public IAsyncCommand<AlbumItemViewModel> Play => commands.PlayAlbumItem;

        public IAsyncCommand<AlbumItemViewModel> AddToPlaylist => commands.AddToPlaylistCommand;

        public IAsyncCommand<AlbumItemViewModel> AddToNowPlaying => commands.AddToNowPlayingCommand;

        public string AlbumArtPath => library.AlbumArts.Exists(Item.Id.Value) ? library.AlbumArts.GetPath(Item.Id.Value) : " ";
    }
}
