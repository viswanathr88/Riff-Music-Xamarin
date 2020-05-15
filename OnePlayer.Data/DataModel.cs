using System;

namespace OnePlayer.Data
{
    public enum DriveItemSource
    {
        OneDrive = 0
    };

    public sealed class Genre
    {
        public long? Id { get; set; }
        public string Name { get; set; }
    }

    public sealed class Artist
    {
        public long? Id { get; set; }
        public string Name { get; set; }
    }

    public sealed class Album
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public int ReleaseYear { get; set; }
        public Artist Artist { get; set; }
        public Genre Genre { get; set; }
    }

    public sealed class Track
    {
        public long? Id { get; set; }
        public string Title { get; set; }
        public int Number { get; set; }
        public string Artist { get; set; }
        public int Bitrate { get; set; }
        public int Duration { get; set; }
        public string Composers { get; set; }
        public int ReleaseYear { get; set; }
        public Album Album { get; set; }
        public Genre Genre { get; set; }
    }

    public sealed class DriveItem
    {
        public string Id { get; set; }
        public string CTag { get; set; }
        public string ETag { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime LastModified { get; set; }
        public string DownloadUrl { get; set; }
        public int Size { get; set; }
        public DriveItemSource Source { get; set; }
        public Track Track { get; set; }
    }

    public class ThumbnailInfo
    {
        public long? Id { get; set; }
        public string SmallUrl { get; set; }
        public string MediumUrl { get; set; }
        public string LargeUrl { get; set; }
        public bool Cached { get; set; }
        public int AttemptCount { get; set; }
        public string DriveItemId { get; set; }
        public long AlbumId { get; set; }
    }
    public class IndexedTrack
    {
        public long? Id { get; set; }
        public string ArtistName { get; set; }
        public string AlbumName { get; set; }
        public string ArtistId { get; set; }
        public string AlbumId { get; set; }
        public string GenreName { get; set; }
        public string GenreId { get; set; }
        public string TrackName { get; set; }
        public string TrackArtist { get; set; }

        public int GetAlbumId()
        {
            return Convert.ToInt32(AlbumId);
        }

        public void SetAlbumId(int id)
        {
            AlbumId = id.ToString();
        }

        public int GetArtistId()
        {
            return Convert.ToInt32(ArtistId);
        }

        public void SetArtistId(int id)
        {
            ArtistId = id.ToString();
        }
        public int GetGenreId()
        {
            return Convert.ToInt32(GenreId);
        }

        public void SetGenreId(int id)
        {
            GenreId = id.ToString();
        }
    }

    public class IndexedTrackWithOffset : IndexedTrack
    {
        public int ColumnIndex { get; set; }
        public int ByteOffset { get; set; }
    }

    public class AlbumQueryItem
    {
        public long Id { get; set; }
        public string AlbumName { get; set; }
        public string ArtistName { get; set; }
        public int Rank { get; set; }
    }

    public class ArtistQueryItem
    {
        public long Id { get; set; }
        public string ArtistName { get; set; }
        public int TrackCount { get; set; }
        public int Rank { get; set; }
    }

    public class GenreQueryItem
    {
        public long Id { get; set; }
        public string GenreName { get; set; }
        public int TrackCount { get; set; }
        public int Rank { get; set; }
    }

    public class TrackQueryItem
    {
        public long Id { get; set; }
        public string TrackName { get; set; }
        public string TrackArtist { get; set; }
        public int Rank { get; set; }
    }


}
