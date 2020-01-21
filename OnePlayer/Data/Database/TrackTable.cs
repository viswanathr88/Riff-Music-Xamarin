using OnePlayer.Data;
using SQLite;
using System.Collections.Generic;

namespace OnePlayer.Database
{
    sealed class TrackTable : ITrackAccessor
    {
        private readonly SQLiteConnection connection;

        public TrackTable(SQLiteConnection connection)
        {
            this.connection = connection;
        }

        public Track Add(Track track)
        {
            this.connection.Insert(track);
            return track;
        }

        public Track Get(int id)
        {
            return connection.Table<Track>().Where(track => track.Id == id).FirstOrDefault();
        }

        public IList<Track> GetAll()
        {
            return connection.Table<Track>().ToList();
        }

        public Track Update(Track track)
        {
            this.connection.Update(track);
            return track;
        }
    }
}
