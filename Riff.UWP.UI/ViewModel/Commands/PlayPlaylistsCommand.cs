using Riff.Data;
using Riff.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Riff.UWP.ViewModel.Commands
{
    public sealed class PlayPlaylistsCommand : ICommand
    {
        private readonly IPlayer player;
        private readonly IMusicLibrary musicLibrary;
        private bool canExecute = true;

        public PlayPlaylistsCommand(IPlayer player, IMusicLibrary musicLibrary)
        {
            this.musicLibrary = musicLibrary;
            this.player = player;
        }

        public bool AddToNowPlayingList
        {
            get;
            set;
        }

        private bool CanExecuteCommand
        {
            get => canExecute;
            set
            {
                if (this.canExecute != value)
                {
                    this.canExecute = value;
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return CanExecuteCommand;
        }

        public async void Execute(object parameter)
        {
            CanExecuteCommand = false;
            IList<DriveItem> tracks = new List<DriveItem>();

            if (parameter != null && parameter is IList<DriveItem> driveItems)
            {
                tracks = driveItems;
            }
            else if (parameter != null && parameter is IList<object> items)
            {
                foreach (var item in items)
                {
                    if (item is Playlist playlist)
                    {
                        ExtractDriveItems(tracks, playlist);
                    }
                }
            }
            else if (parameter != null && parameter is Playlist playlist)
            {
                ExtractDriveItems(tracks, playlist);
            }

            if (tracks.Count > 0)
            {
                await player.PlayAsync(tracks, 0, autoplay: !AddToNowPlayingList);
            }

            CanExecuteCommand = true;
        }

        private void ExtractDriveItems(IList<DriveItem> tracks, Playlist playlist)
        {
            var options = new PlaylistItemAccessOptions()
            {
                PlaylistFilter = playlist.Id,
                IncludeDriveItem = true
            };
            var playlistItems = musicLibrary.PlaylistItems.Get(options);
            foreach (var playlistItem in playlistItems)
            {
                tracks.Add(playlistItem.DriveItem);
            }
        }
    }
}
