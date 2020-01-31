using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace OnePlayer.Data.Sqlite
{
    sealed class IndexedTrackTable : IIndexAccessor
    {
        private readonly SQLiteConnection connection;

        public IndexedTrackTable(SQLiteConnection conn)
        {
            connection = conn;
        }

        public IndexedTrack Get(int id)
        {
            string query = $"SELECT docid AS Id, * FROM {nameof(IndexedTrack)} WHERE docid = {id};";
            return connection.Query<IndexedTrack>(query).FirstOrDefault();
        }

        public IndexedTrack Add(IndexedTrack track)
        {
            string query = $"INSERT INTO {nameof(IndexedTrack)}({string.Join(",", GetFieldsWithId())}) VALUES ({string.Join(",", GetValues(track))})";
            connection.Execute(query);
            return track;
        }

        public void EnsureCreated()
        {
            string query = $"CREATE VIRTUAL TABLE IF NOT EXISTS {nameof(IndexedTrack)} USING fts4({string.Join(",", GetFields())})";
            connection.Execute(query);
        }

        public IList<IndexedTrackWithOffset> Search(string term)
        {
            string query = $"SELECT docid AS Id,*,substr(offsets({nameof(IndexedTrack)}),1,1) AS ColumnIndex, substr(offsets({nameof(IndexedTrack)}),5,2) AS ByteOffset FROM {nameof(IndexedTrack)} WHERE {nameof(IndexedTrack)} MATCH \"{term}*\" ORDER BY ColumnIndex ASC, ByteOffset ASC";
            return connection.Query<IndexedTrackWithOffset>(query).ToList();
        }

        public IndexedTrack Update(IndexedTrack track)
        {
            var fields = GetFieldsWithId();
            var values = GetValues(track);
            string[] columns = new string[fields.Length];
            for (int i = 0; i < fields.Length; i++)
            {
                columns[i] = $"{fields[i]} = {values[i]}";
            }

            string query = $"UPDATE TABLE {nameof(IndexedTrack)} SET {string.Join(",", columns)};";
            connection.Execute(query);
            return track;
        }

        public IList<AlbumQueryItem> FindMatchingAlbums(string term, int? maxCount)
        {
            List<AlbumQueryItem> albums = new List<AlbumQueryItem>();
            if (!string.IsNullOrEmpty(term))
            {
                string limit = maxCount.HasValue ? $"LIMIT {maxCount.Value}" : string.Empty;
                string query = $"SELECT Id, AlbumName, ArtistName, Rank FROM " +
                    $"(SELECT CAST(AlbumId AS INTEGER) AS Id, AlbumName, ArtistName, CAST(substr(offsets({nameof(IndexedTrack)}), 5, 2) AS INTEGER) AS Rank " +
                    $"FROM {nameof(IndexedTrack)} " +
                    $"WHERE AlbumName MATCH \"{term}*\" ORDER BY RANK ASC) " +
                    $"GROUP BY Id, AlbumName, ArtistName, Rank " +
                    $"ORDER BY Rank ASC, AlbumName ASC {limit}";
                albums = connection.Query<AlbumQueryItem>(query);
            }

            return albums;
        }

        public IList<ArtistQueryItem> FindMatchingArtists(string term, int? maxCount)
        {
            List<ArtistQueryItem> artists = new List<ArtistQueryItem>();

            if (!string.IsNullOrEmpty(term))
            {
                string limit = maxCount.HasValue ? $"LIMIT {maxCount.Value}" : string.Empty;
                string query = $"SELECT Id, ArtistName, COUNT(ArtistName) AS TrackCount, Rank FROM " +
                    $"(SELECT CAST(ArtistId AS INTEGER) AS Id, ArtistName, CAST(substr(offsets({nameof(IndexedTrack)}), 5, 2) AS INTEGER) AS Rank FROM {nameof(IndexedTrack)} " +
                    $"WHERE ArtistName MATCH \"{term}*\" ORDER BY RANK ASC) " +
                    $"GROUP BY Id, ArtistName, Rank " +
                    $"ORDER BY Rank ASC, ArtistName ASC {limit}";
                artists = connection.Query<ArtistQueryItem>(query);
            }

            return artists;
        }

        public IList<GenreQueryItem> FindMatchingGenres(string term, int? maxCount)
        {
            List<GenreQueryItem> genres = new List<GenreQueryItem>();

            if (!string.IsNullOrEmpty(term))
            {
                string limit = maxCount.HasValue ? $"LIMIT {maxCount.Value}" : string.Empty;
                string query = $"SELECT Id, GenreName, COUNT(GenreName) AS TrackCount, Rank FROM " +
                    $"(SELECT CAST(GenreId AS INTEGER) AS Id, GenreName, CAST(substr(offsets({nameof(IndexedTrack)}), 5, 2) AS INTEGER) AS Rank FROM {nameof(IndexedTrack)} " +
                    $"WHERE GenreName MATCH \"{term}*\" ORDER BY RANK ASC) " +
                    $"GROUP BY Id, GenreName, Rank " +
                    $"ORDER BY Rank ASC, GenreName ASC {limit}";
                genres = connection.Query<GenreQueryItem>(query);
            }

            return genres;
        }

        public IList<TrackQueryItem> FindMatchingTracks(string term, int? maxCount)
        {
            List<TrackQueryItem> tracks = new List<TrackQueryItem>();

            if (!string.IsNullOrEmpty(term))
            {
                {
                    string limit = maxCount.HasValue ? $"LIMIT {maxCount.Value}" : string.Empty;
                    string query = $"SELECT docid AS Id, TrackName, TrackArtist, CAST(substr(offsets({nameof(IndexedTrack)}), 5, 2) AS INTEGER) AS Rank " +
                        $"FROM {nameof(IndexedTrack)} WHERE TrackName MATCH \"{term}*\" " +
                        $"ORDER BY RANK ASC, TrackName ASC " +
                        $"{limit}";
                    tracks = connection.Query<TrackQueryItem>(query);
                }
            }

            return tracks;
        }

        public IList<TrackQueryItem> FindMatchingTracksWithArtists(string term, int? maxCount)
        {
            List<TrackQueryItem> tracks = new List<TrackQueryItem>();

            if (!string.IsNullOrEmpty(term))
            {
                string limit = maxCount.HasValue ? $"LIMIT {maxCount.Value}" : string.Empty;
                string query = $"SELECT docid AS Id, TrackName, TrackArtist, CAST(substr(offsets({nameof(IndexedTrack)}), 5, 2) AS INTEGER) AS Rank " +
                    $"FROM {nameof(IndexedTrack)} WHERE TrackArtist MATCH \"{term}*\" " +
                    $"ORDER BY RANK ASC, TrackName ASC " +
                    $"{limit}";
                tracks = connection.Query<TrackQueryItem>(query);
            }

            return tracks;
        }

        private string[] GetFields()
        {
            return new string[] 
            {
                nameof(IndexedTrack.ArtistName), 
                nameof(IndexedTrack.AlbumName), 
                nameof(IndexedTrack.TrackName), 
                nameof(IndexedTrack.TrackArtist), 
                nameof(IndexedTrack.GenreName),
                nameof(IndexedTrack.AlbumId),
                nameof(IndexedTrack.ArtistId),
                nameof(IndexedTrack.GenreId)
            };
        }

        private string[] GetFieldsWithId()
        {
            return new string[]
            {
                "docid",
                nameof(IndexedTrack.ArtistName),
                nameof(IndexedTrack.AlbumName),
                nameof(IndexedTrack.TrackName),
                nameof(IndexedTrack.TrackArtist),
                nameof(IndexedTrack.GenreName),
                nameof(IndexedTrack.AlbumId),
                nameof(IndexedTrack.ArtistId),
                nameof(IndexedTrack.GenreId)
            };
        }

        private string[] GetValues(IndexedTrack track)
        {
            var values = new string[9];
            values[0] = track.Id.ToString();
            values[1] = $"\"{track.ArtistName}\"";
            values[2] = $"\"{track.AlbumName}\"";
            values[3] = $"\"{track.TrackName}\"";
            values[4] = $"\"{track.TrackArtist}\"";
            values[5] = $"\"{track.GenreName}\"";
            values[6] = $"\"{track.AlbumId}\"";
            values[7] = $"\"{track.ArtistId}\"";
            values[8] = $"\"{track.GenreId}\"";

            return values;
        }
    }
}
