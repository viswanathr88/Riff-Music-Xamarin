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
        private readonly IMusicDataAccessor musicDataAccessor;

        public MusicDataStore() : this(DefaultPath)
        {
            this.musicDataAccessor = new Sqlite.MusicDb(dbPath);
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

            TrackThumbnails = new ThumbnailCache(Path.Combine(thumbnailPath, "Tracks"));
            AlbumThumbnails = new ThumbnailCache(Path.Combine(thumbnailPath, "Albums"));
        }
        public IMusicDataAccessor Access()
        {
            return this.musicDataAccessor;
        }

        public IThumbnailCache TrackThumbnails { get; }

        public IThumbnailCache AlbumThumbnails { get; }
    }
}
