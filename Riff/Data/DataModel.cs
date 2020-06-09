using System;
using System.Collections.Generic;

namespace Riff.Data
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
        public TimeSpan Duration { get; set; }
        public string Composers { get; set; }
        public int ReleaseYear { get; set; }
        public Album Album { get; set; }
        public Genre Genre { get; set; }

        public string FormatDuration(TimeSpan duration)
        {
            if (duration.Days > 0)
            {
                return duration.ToString("dd':'hh':'mm':'ss");
            }
            else if (duration.Hours > 0)
            {
                return duration.ToString("hh':'mm':'ss");
            }
            else
            {
                return duration.ToString("mm':'ss");
            }
        }
    }

    public sealed class DriveItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
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
    }
    public class IndexedTrack
    {
        public long? Id { get; set; }
        public string FileName { get; set; }
        public string ArtistName { get; set; }
        public string AlbumName { get; set; }
        public string ArtistId { get; set; }
        public string AlbumId { get; set; }
        public string GenreName { get; set; }
        public string GenreId { get; set; }
        public string TrackName { get; set; }
        public string TrackArtist { get; set; }

        public long GetAlbumId()
        {
            return Convert.ToInt64(AlbumId);
        }

        public void SetAlbumId(long id)
        {
            AlbumId = id.ToString();
        }

        public long GetArtistId()
        {
            return Convert.ToInt64(ArtistId);
        }

        public void SetArtistId(long id)
        {
            ArtistId = id.ToString();
        }
        public long GetGenreId()
        {
            return Convert.ToInt64(GenreId);
        }

        public void SetGenreId(long id)
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
        public long AlbumId { get; set; }
    }

    public enum SearchItemType
    {
        Artist,
        Album,
        Genre,
        Track,
        TrackArtist
    };

    public class SearchItem
    {
        public long Id { get; set; }
        public SearchItemType Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Rank { get; set; }
        public long? ParentId { get; set; }
    }

    public sealed class SearchQuery
    {
        public SearchQuery()
        {
            // Defaults
            MaxArtistCount = 3;
            MaxGenreCount = 2;
            MaxAlbumCount = 5;
            MaxTrackCount = 15;
        }

        public string Term { get; set; }

        public int MaxArtistCount { get; set; }

        public int MaxGenreCount { get; set; }

        public int MaxAlbumCount { get; set; }

        public int MaxTrackCount { get; set; }
    }

    public sealed class SearchResult
    {
        private readonly List<SearchItem> artists = new List<SearchItem>();
        private readonly List<SearchItem> albums = new List<SearchItem>();
        private readonly List<SearchItem> genres = new List<SearchItem>();
        private readonly List<SearchItem> tracks = new List<SearchItem>();

        public IList<SearchItem> Artists => artists;

        public IList<SearchItem> Albums => albums;

        public IList<SearchItem> Genres => genres;

        public IList<SearchItem> Tracks => tracks;
    }


}
