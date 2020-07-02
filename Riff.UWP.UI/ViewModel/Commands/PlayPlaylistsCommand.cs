using Riff.Data;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Riff.UWP.ViewModel.Commands
{
    public sealed class PlayPlaylistsCommand : ICommand
    {
        private readonly IPlayer player;

        public PlayPlaylistsCommand(IPlayer player)
        {
            this.player = player;
        }

        public bool AddToNowPlayingList
        {
            get;
            set;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            List<DriveItem> tracks = new List<DriveItem>();

            if (parameter != null && parameter is IList<object> items)
            {
                foreach (var item in items)
                {
                    if (item is Playlist playlist)
                    {
                        await playlist.LoadAsync();
                        tracks.AddRange(playlist.Items);
                    }
                }
            }
            else if (parameter != null && parameter is Playlist playlist)
            {
                await playlist.LoadAsync();
                tracks.AddRange(playlist.Items);
            }

            if (tracks.Count > 0)
            {
                await player.PlayAsync(tracks, 0, autoplay: !AddToNowPlayingList);
            }
        }

        private void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
