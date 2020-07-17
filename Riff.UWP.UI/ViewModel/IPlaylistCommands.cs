using Mirage.ViewModel.Commands;
using Riff.Data;
using System.Collections.Generic;

namespace Riff.UWP.ViewModel
{
    public interface IPlaylistCommands
    {
        /// <summary>
        /// Gets the command to play a playlist
        /// </summary>
        IAsyncCommand<Playlist> Play { get; }
        /// <summary>
        /// Gets the command to play drive items
        /// </summary>
        IAsyncCommand<IList<DriveItem>> PlayItems { get; }
        /// <summary>
        /// Gets the command to add a playlist to now playing
        /// </summary>
        IAsyncCommand<Playlist> PlayNext { get; }
        /// <summary>
        /// Gets the command to add a playlist to now playing
        /// </summary>
        IAsyncCommand<IList<DriveItem>> PlayItemsNext { get; }
    }
}
