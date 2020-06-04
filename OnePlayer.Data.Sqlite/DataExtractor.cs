using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace OnePlayer.Data.Sqlite
{
    public sealed class DataExtractor
    {
        private readonly IDictionary<long, Artist> artistCache = new Dictionary<long, Artist>();
        private readonly IDictionary<long, Genre> genreCache = new Dictionary<long, Genre>();
        private readonly IDictionary<long, Album> albumCache = new Dictionary<long, Album>();
        private readonly IDictionary<long, Track> trackCache = new Dictionary<long, Track>();

        public bool DisableCache { get; set; } = true;

        public Artist ExtractArtist(SqliteDataReader reader)
        {
            int index = 0;
            return ExtractArtist(reader, ref index);
        }

        private Artist ExtractArtist(SqliteDataReader reader, ref int index)
        {
            var artist = new Artist()
            {
                Id = (index < reader.FieldCount && reader.GetName(index) == "ArtistId") ? (long?)reader.GetInt64(index++) : null,
                Name = (index < reader.FieldCount && reader.GetName(index) == "ArtistName" && !reader.IsDBNull(index++)) ? reader.GetString(index-1) : null
            };

            if (artist.Id == null)
            {
                return null;
            }

            if (!DisableCache)
            {
                if (artistCache.ContainsKey(artist.Id.Value) && string.Compare(artistCache[artist.Id.Value].Name, artist.Name) >= 0)
                {
                    artist = artistCache[artist.Id.Value];
                }
                else
                {
                    artistCache[artist.Id.Value] = artist;
                }
            }

            return artist;
        }

        public Genre ExtractGenre(SqliteDataReader reader)
        {
            int index = 0;
            return ExtractGenre(reader, ref index);
        }

        private Genre ExtractGenre(SqliteDataReader reader, ref int index, string fieldPrefix = "")
        {
            var genre = new Genre()
            {
                Id = (index < reader.FieldCount && reader.GetName(index) == $"{fieldPrefix}GenreId") ? (long?)reader.GetInt64(index++) : null,
                Name = (index < reader.FieldCount && reader.GetName(index) == $"{fieldPrefix}GenreName" && !reader.IsDBNull(index++)) ? reader.GetString(index-1) : null
            };

            if (genre.Id == null)
            {
                return null;
            }

            if (!DisableCache)
            {
                if (genreCache.ContainsKey(genre.Id.Value) && string.Compare(genreCache[genre.Id.Value].Name, genre.Name) >= 0)
                {
                    genre = genreCache[genre.Id.Value];
                }
                else
                {
                    genreCache[genre.Id.Value] = genre;
                }
            }

            return genre;
        }

        public Album ExtractAlbum(SqliteDataReader reader)
        {
            int index = 0;
            return ExtractAlbum(reader, ref index);
        }

        private Album ExtractAlbum(SqliteDataReader reader, ref int index)
        {
            var album = new Album()
            {
                Id = (index < reader.FieldCount && reader.GetName(index) == "AlbumId") ? (long?)reader.GetInt64(index++) : null,
                Name = (index < reader.FieldCount && reader.GetName(index) == "AlbumName" && !reader.IsDBNull(index++)) ? reader.GetString(index-1) : null,
                ReleaseYear = (index < reader.FieldCount && reader.GetName(index) == "AlbumReleaseYear") ? reader.GetInt32(index++) : 0,
                Artist = ExtractArtist(reader, ref index),
                Genre = ExtractGenre(reader, ref index)
            };

            if (album.Id == null)
            {
                return null;
            }

            if (!DisableCache)
            {
                if (albumCache.ContainsKey(album.Id.Value) && string.Compare(albumCache[album.Id.Value].Name, album.Name) >= 0)
                {
                    album = albumCache[album.Id.Value];
                }
                else
                {
                    albumCache[album.Id.Value] = album;
                }
            }

            return album;
        }

        public Track ExtractTrack(SqliteDataReader reader)
        {
            int index = 0;
            return ExtractTrack(reader, ref index);
        }

        private Track ExtractTrack(SqliteDataReader reader, ref int index)
        {
            var track = new Track()
            {
                Id = (index < reader.FieldCount && reader.GetName(index) == "TrackId") ? (long?)reader.GetInt64(index++) : null,
                Title = (index < reader.FieldCount && reader.GetName(index) == "TrackTitle" && !reader.IsDBNull(index++)) ? reader.GetString(index-1) : null,
                Number = (index < reader.FieldCount && reader.GetName(index) == "TrackNumber") ? reader.GetInt32(index++) : 0,
                Artist = (index < reader.FieldCount && reader.GetName(index) == "TrackArtist" && !reader.IsDBNull(index++)) ? reader.GetString(index-1) : null,
                Bitrate = (index < reader.FieldCount && reader.GetName(index) == "TrackBitrate") ? reader.GetInt32(index++) : 0,
                Duration = (index < reader.FieldCount && reader.GetName(index) == "TrackDuration") ? TimeSpan.FromMilliseconds(reader.GetInt32(index++)) : TimeSpan.Zero,
                Composers = (index < reader.FieldCount && reader.GetName(index) == "TrackComposers" && !reader.IsDBNull(index++)) ? reader.GetString(index-1) : null,
                ReleaseYear = (index < reader.FieldCount && reader.GetName(index) == "TrackReleaseYear") ? reader.GetInt32(index++) : 0,
                Album = ExtractAlbum(reader, ref index),
                Genre = ExtractGenre(reader, ref index, "Track")
            };

            if (track.Id == null)
            {
                return null;
            }

            if (!DisableCache)
            {
                if (trackCache.ContainsKey(track.Id.Value) && string.Compare(trackCache[track.Id.Value].Title, track.Title) >= 0)
                {
                    track = trackCache[track.Id.Value];
                }
                else
                {
                    trackCache[track.Id.Value] = track;
                }
            }

            return track;
        }

        public DriveItem ExtractDriveItem(SqliteDataReader reader)
        {
            int index = 0;
            return ExtractDriveItem(reader, ref index);
        }

        private DriveItem ExtractDriveItem(SqliteDataReader reader, ref int index)
        {
            var item = new DriveItem()
            {
                Id = (index < reader.FieldCount && reader.GetName(index) == "DriveItemId") ? reader.GetString(index++) : null,
                CTag = (index < reader.FieldCount && reader.GetName(index) == "DriveItemCTag" && !reader.IsDBNull(index++)) ? reader.GetString(index-1) : null,
                ETag = (index < reader.FieldCount && reader.GetName(index) == "DriveItemETag" && !reader.IsDBNull(index++)) ? reader.GetString(index-1) : null,
                AddedDate = (index < reader.FieldCount && reader.GetName(index) == "DriveItemAddedDate") ? new DateTime(reader.GetInt64(index++)) : DateTime.MinValue,
                LastModified = (index < reader.FieldCount && reader.GetName(index) == "DriveItemLastModified") ? new DateTime(reader.GetInt64(index++)) : DateTime.MinValue,
                DownloadUrl = (index < reader.FieldCount && reader.GetName(index) == "DriveItemDownloadUrl" && !reader.IsDBNull(index++)) ? reader.GetString(index-1) : null,
                Size = (index < reader.FieldCount && reader.GetName(index) == "DriveItemSize") ? reader.GetInt32(index++) : 0,
                Source = (index < reader.FieldCount && reader.GetName(index) == "DriveItemSource") ? (DriveItemSource)reader.GetInt32(index++) : DriveItemSource.OneDrive,
                Track = ExtractTrack(reader, ref index)
            };

            return item;
        }

    }
}