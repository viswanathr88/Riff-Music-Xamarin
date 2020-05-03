using OnePlayer.Data.Sqlite;
using System;
using System.IO;

namespace OnePlayer.Data
{
    public sealed class MusicDataStore : IMusicDataStore
    {
        private readonly string rootPath;
        private readonly string dbPath;
        private readonly string thumbnailPath;
        private readonly IMusicDataAccessor musicDataAccessor;

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
            musicDataAccessor = new MusicDb(dbPath);

            TrackThumbnails = new ThumbnailCache(Path.Combine(thumbnailPath, "Tracks"), Path.Combine("Thumbnails", "Tracks"));
            AlbumThumbnails = new ThumbnailCache(Path.Combine(thumbnailPath, "Albums"), Path.Combine("Thumbnails", "Albums"));
        }
        public IMusicDataAccessor Access()
        {
            return this.musicDataAccessor;
        }

        public IThumbnailCache TrackThumbnails { get; }

        public IThumbnailCache AlbumThumbnails { get; }
    }
}
