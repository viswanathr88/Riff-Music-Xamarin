using System;
using System.Collections.Generic;
using System.Net.Http;

namespace OnePlayer.Data
{
    public sealed class MusicLibrary : IDisposable
    {
        private readonly IMusicDataStore store;
        private readonly IMusicDataAccessor data;
        private readonly MusicLibraryWriter writer;

        public MusicLibrary(IMusicDataStore dataStore, HttpClient httpClient)
        {
            store = dataStore ?? throw new ArgumentNullException(nameof(dataStore));

            data = dataStore.Access();
            data.Migrate();

            writer = new MusicLibraryWriter(dataStore, httpClient);
        }

        public IThumbnailCache AlbumArts => store.AlbumThumbnails;

        public IThumbnailCache TrackArts => store.TrackThumbnails;

        public event EventHandler<DriveItem> ItemAdded
        {
            add { writer.ItemAdded += value; }
            remove { writer.ItemRemoved -= value; }
        }
        
        public event EventHandler<DriveItem> ItemRemoved
        {
            add { writer.ItemRemoved += value; }
            remove { writer.ItemRemoved -= value; }
        }

        public event EventHandler<DriveItem> ItemModified
        {
            add { writer.ItemModified += value; }
            remove { writer.ItemModified -= value; }
        }

        public IList<Artist> GetArtists()
        {
            return data.Artists.GetAll();
        }

        public IList<Album> GetAlbums()
        {
            return data.Albums.GetAll();
        }

        public IList<Track> GetTracks()
        {
            return data.Tracks.GetAll();
        }

        public IList<Genre> GetGenres()
        {
            return data.Genres.GetAll();
        }

        public void Search(SearchQuery query, List<SearchItem> results)
        {
            foreach (var artist in data.Index.FindMatchingArtists(query.Term, query.MaxArtistCount))
            {
                results.Add(new SearchItem { Id = artist.Id, Type = SearchItemType.Artist, Name = artist.ArtistName, Description = artist.TrackCount.ToString(), Rank = artist.Rank });
            }

            foreach (var genre in data.Index.FindMatchingGenres(query.Term, query.MaxGenreCount))
            {
                results.Add(new SearchItem { Id = genre.Id, Type = SearchItemType.Genre, Name = genre.GenreName, Description = genre.TrackCount.ToString(), Rank = genre.Rank });
            }

            foreach (var album in data.Index.FindMatchingAlbums(query.Term, query.MaxAlbumCount))
            {
                results.Add(new SearchItem() { Id = album.Id, Type = SearchItemType.Album, Name = album.AlbumName, Description = album.ArtistName, Rank = album.Rank });
            }

            var tracks = data.Index.FindMatchingTracks(query.Term, query.MaxTrackCount);
            foreach (var track in data.Index.FindMatchingTracks(query.Term, query.MaxTrackCount))
            {
                results.Add(new SearchItem() { Id = track.Id, Type = SearchItemType.Track, Name = track.TrackName, Description = track.TrackArtist, Rank = track.Rank });
            }

            if (query.MaxTrackCount > tracks.Count)
            {
                int diff = query.MaxTrackCount - tracks.Count;
                foreach (var track in data.Index.FindMatchingTracksWithArtists(query.Term, diff))
                {
                    results.Add(new SearchItem() { Id = track.Id, Type = SearchItemType.TrackArtist, Name = track.TrackName, Description = track.TrackArtist, Rank = track.Rank });
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

        internal MusicLibraryWriter Edit()
        {
            return writer;
        }

        public void Dispose()
        {
            data?.Dispose();
        }
    }
}
