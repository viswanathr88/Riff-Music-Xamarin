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

    public class ThumbnailInfo
    {
        [PrimaryKey]
        public int Id { get; set; }
        [Indexed]
        public string DriveItemId { get; set; }
        public string SmallUrl { get; set; }
        public string MediumUrl { get; set; }
        public string LargeUrl { get; set; }
        [Indexed]
        public bool Cached { get; set; }
        [Indexed]
        public int AttemptCount { get; set; }
    }

    public class IndexedTrack
    {
        public int Id { get; set; }
        public string ArtistName { get; set; }
        public string AlbumName { get; set; }
        public string GenreName { get; set; }
        public string TrackName { get; set; }
        public string TrackArtist { get; set; }
    }

    public class IndexedTrackWithOffset : IndexedTrack
    {
        public int ColumnIndex { get; set; }

        public int ByteOffset { get; set; }
    }

    public class AlbumQueryItem
    {
        public string AlbumName { get; set; }

        public string ArtistName { get; set; }

        public int Rank { get; set; }
    }

    public class ArtistQueryItem
    {
        public string ArtistName { get; set; }

        public int TrackCount { get; set; }

        public int Rank { get; set; }
    }

    public class GenreQueryItem
    {
        public string GenreName { get; set; }

        public int TrackCount { get; set; }

        public int Rank { get; set; }
    }

    public class TrackQueryItem
    {
        public int Id { get; set; }
        
        public string TrackName { get; set; }

        public string TrackArtist { get; set; }

        public int Rank { get; set; }
    }

    public enum DriveItemSource
    {
        OneDrive = 0
    };
}
