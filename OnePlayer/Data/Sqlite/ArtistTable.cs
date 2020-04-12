using SQLite;
using System.Collections.Generic;

namespace OnePlayer.Data.Sqlite
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
            this.connection.Insert(artist);
            return artist;
        }

        public void EnsureCreated()
        {
            connection.CreateTable<Artist>();
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
            return connection.Table<Artist>().OrderBy(artist => artist.Name).ToList();
        }

        public Artist Update(Artist artist)
        {
            connection.Update(artist);
            return artist;
        }
    }
}
