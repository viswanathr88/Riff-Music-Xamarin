using System;
using System.Collections.Generic;

namespace Riff.Data
{
    public interface IPlaylistManager
    {
        Playlist GetNowPlayingList();

        IList<Playlist> GetPlaylists();

        Playlist CreatePlaylist(string name);

        void DeletePlaylist(string name);

        void RenamePlaylist(string oldName, string newName);

        event EventHandler<EventArgs> StateChanged;
    }
}
