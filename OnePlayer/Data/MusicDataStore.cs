using System;
using System.IO;

namespace OnePlayer.Data
{
    sealed class MusicDataStore : IMusicDataStore
    {
        private static readonly string DefaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
        private readonly string rootPath;
        private readonly string dbPath;
        private readonly string thumbnailPath;

        public MusicDataStore() : this(DefaultPath)
        {

        }

        public MusicDataStore(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(dbPath));
            }

            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException($"{path} does not exist");
            }

            rootPath = path;
            dbPath = Path.Combine(this.rootPath, "oneplayer.db");
            thumbnailPath = Path.Combine(this.rootPath, "Thumbnails");

            Thumbnails = new ThumbnailCache(thumbnailPath);
        }
        public IMusicDataAccessor Create()
        {
            return new Sqlite.MusicDb(dbPath);
        }

        public IThumbnailCache Thumbnails { get; }
    }
}
