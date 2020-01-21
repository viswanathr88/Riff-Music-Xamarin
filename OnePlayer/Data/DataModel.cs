using SQLite;
using System;

namespace OnePlayer.Data
{
    public class Genre
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        [Indexed]
        public string NameLower { get; set; }
    }

    public class Artist
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        [Indexed]
        public string NameLower { get; set; }
    }

    public class Album
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        [Indexed]
        public string NameLower { get; set; }
        [Indexed]
        public int ArtistId { get; set; }
        [Indexed]
        public int ReleaseYear { get; set; }
        [Indexed]
        public int GenreId { get; set; }
    }

    public class Track
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Title { get; set; }
        [Indexed]
        public string TitleLower { get; set; }
        [Indexed]
        public int Number { get; set; }
        [Indexed]
        public int AlbumId { get; set; }
        [Indexed]
        public int GenreId { get; set; }
        [Indexed]
        public string Artist { get; set; }
        [Indexed]
        public int Bitrate { get; set; }

        public int Duration { get; set; }
        [Indexed]
        public string Composers { get; set; }
        [Indexed]
        public int ReleaseYear { get; set; }
    }

    public class DriveItem
    {
        [PrimaryKey]
        public string Id { get; set; }
        [Indexed]
        public string CTag { get; set; }
        [Indexed]
        public string ETag { get; set; }
        [Indexed]
        public int TrackId { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime LastModified { get; set; }
        public string DownloadUrl { get; set; }
        public int Size { get; set; }
        [Indexed]
        public DriveItemSource Source { get; set; }
    }

    public enum DriveItemSource
    {
        OneDrive = 0
    };
}
