using System;
using System.Collections.Generic;
using System.IO;

namespace Riff.Data
{
    internal sealed class PlaylistManager : IPlaylistManager
    {
        private readonly string rootPath;
        private readonly static string nowPlaylingListKey = "ED65F63B-1648-4F81-8579-76C45C6E4EA4";
        private readonly static string tempFileKey = "0B439A52-D984-4C30-975E-D5DC11AEEA23";

        public event EventHandler<EventArgs> StateChanged;

        public PlaylistManager(string rootPath)
        {
            this.rootPath = rootPath;

            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }

            string path = Path.Combine(rootPath, nowPlaylingListKey);
            if (!File.Exists(path))
            {
                using (var stream = File.Create(path))
                {

                }
            }
        }

        public Playlist CreatePlaylist(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            string path = Path.Combine(rootPath, name);
            if (File.Exists(path))
            {
                throw new Exception($"Playlist {name} already exists");
            }

            using (var stream = File.Create(path))
            {
            }

            var playlist = new Playlist(rootPath, name);
            StateChanged?.Invoke(this, EventArgs.Empty);
            return playlist;
        }

        public void DeletePlaylist(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            string path = Path.Combine(rootPath, name);
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            StateChanged?.Invoke(this, EventArgs.Empty);
        }

        public Playlist GetNowPlayingList()
        {
            return new Playlist(rootPath, nowPlaylingListKey);
        }

        public IList<Playlist> GetPlaylists()
        {
            IList<Playlist> lists = new List<Playlist>();
            string[] files = Directory.GetFiles(rootPath);

            foreach (var file in files)
            {
                string name = Path.GetFileNameWithoutExtension(file);
                if (name != nowPlaylingListKey)
                {
                    lists.Add(new Playlist(rootPath, name));
                }
            }

            return lists;
        }

        public void RenamePlaylist(string oldName, string newName)
        {
            if (string.IsNullOrEmpty(oldName) || string.IsNullOrEmpty(newName))
            {
                throw new ArgumentNullException();
            }

            string oldPath = Path.Combine(rootPath, oldName);
            string newPath = Path.Combine(rootPath, newName);


            if (!File.Exists(oldPath))
            {
                throw new UnauthorizedAccessException($"Playlist {oldPath} doesn't exist");
            }

            if (File.Exists(newPath))
            {
                if (string.Compare(oldName, newName, true) == 0)
                {
                    string tempPath = Path.Combine(rootPath, tempFileKey);
                    File.Move(oldPath, tempPath);
                    oldPath = tempPath;
                }
                else
                {
                    throw new UnauthorizedAccessException($"Playlist {newName} already exists");
                }
            }

            File.Move(oldPath, newPath);
            StateChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
