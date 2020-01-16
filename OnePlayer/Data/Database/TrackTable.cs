using OnePlayer.Data;
using SQLite;
using System;
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
            if (track.Id.HasValue)
            {
                throw new Exception("Id cannot have a value");
            }
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
            if (!track.Id.HasValue)
            {
                throw new Exception("Id does have a value");
            }
            this.connection.Update(track);
            return track;
        }
    }
}
