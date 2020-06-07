using Riff.Data.Access;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

namespace Riff.Data
{
    public sealed class MusicLibrary : IDisposable
    {
        public MusicLibrary(string path, IMusicMetadata metadata, HttpClient httpClient)
        {
            Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
            AlbumArts = new ThumbnailCache(Path.Combine(path, "Thumbnails", "Albums"));
        }

        public IMusicMetadata Metadata { get; }

        public ThumbnailCache AlbumArts { get; }

        public IList<Album> GetAlbums()
        {
            var options = new AlbumAccessOptions()
            {
                IncludeArtist = true,
                SortType = AlbumSortType.ReleaseYear,
                SortOrder = SortOrder.Descending
            };

            return Metadata.Albums.Get(options);
        }

        public IList<Artist> GetArtists()
        {
            return Metadata.Artists.GetAll();
        }

        public IList<Track> GetTracks()
        {
            var options = new TrackAccessOptions()
            {
                IncludeAlbum = true,
                IncludeGenre = true,
                SortType = TrackSortType.Title,
                SortOrder = SortOrder.Ascending
            };

            return Metadata.Tracks.Get(options);
        }

        public IList<Genre> GetGenres()
        {
            return Metadata.Genres.GetAll();
        }

        public void Search(SearchQuery query, List<SearchItem> results)
        {
            foreach (var artist in Metadata.Index.FindMatchingArtists(query.Term, query.MaxArtistCount))
            {
                results.Add(new SearchItem { Id = artist.Id, Type = SearchItemType.Artist, Name = artist.ArtistName, Description = artist.TrackCount.ToString(), Rank = artist.Rank });
            }

            foreach (var genre in Metadata.Index.FindMatchingGenres(query.Term, query.MaxGenreCount))
            {
                results.Add(new SearchItem { Id = genre.Id, Type = SearchItemType.Genre, Name = genre.GenreName, Description = genre.TrackCount.ToString(), Rank = genre.Rank });
            }

            foreach (var album in Metadata.Index.FindMatchingAlbums(query.Term, query.MaxAlbumCount))
            {
                results.Add(new SearchItem() { Id = album.Id, Type = SearchItemType.Album, Name = album.AlbumName, Description = album.ArtistName, Rank = album.Rank });
            }
             
            var tracks = Metadata.Index.FindMatchingTracks(query.Term, query.MaxTrackCount);
            foreach (var track in Metadata.Index.FindMatchingTracks(query.Term, query.MaxTrackCount))
            {
                results.Add(new SearchItem() { Id = track.Id, Type = SearchItemType.Track, Name = track.TrackName, Description = track.TrackArtist, Rank = track.Rank, ParentId = track.AlbumId });
            }

            if (query.MaxTrackCount > tracks.Count)
            {
                int diff = query.MaxTrackCount - tracks.Count;
                foreach (var track in Metadata.Index.FindMatchingTracksWithArtists(query.Term, diff))
                {
                    results.Add(new SearchItem() { Id = track.Id, Type = SearchItemType.TrackArtist, Name = track.TrackName, Description = track.TrackArtist, Rank = track.Rank, ParentId = track.AlbumId });
                }
            }

            results.Sort(new SearchItemComparer());
        }

        public IList<SearchItem> Search(SearchQuery query)
        {
            List<SearchItem> searchItems = new List<SearchItem>();
            Search(query, searchItems);
            return searchItems;
        }

        public void Dispose()
        {
            Metadata?.Dispose();
        }
    }
}
