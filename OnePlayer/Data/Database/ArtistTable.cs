using OnePlayer.Data;
using SQLite;
using System;
using System.Collections.Generic;

namespace OnePlayer.Database
{
    sealed class ArtistTable : IArtistAccessor
    {
        private readonly SQLiteConnection connection;

        public ArtistTable(SQLiteConnection connection)
        {
            this.connection = connection;
        }

        public Artist Add(Artist artist)
        {
            if (artist.Id.HasValue)
            {
                throw new Exception("Id cannot have a value");
            }

            this.connection.Insert(artist);
            return artist;
        }

        public Artist Find(string artistName)
        {
            string artistNameLower = artistName.ToLower();
            return connection.Table<Artist>().Where(artist => artist.NameLower == artistNameLower).FirstOrDefault();
        }

        public Artist Get(int id)
        {
            return connection.Table<Artist>().Where(artist => artist.Id == id).FirstOrDefault();
        }

        public IList<Artist> GetAll()
        {
            return connection.Table<Artist>().ToList();
        }

        public Artist Update(Artist artist)
        {
            if (!artist.Id.HasValue)
            {
                throw new Exception("Id does not have a value");
            }

            connection.Update(artist);
            return artist;
        }
    }
}
