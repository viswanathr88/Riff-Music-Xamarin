using OnePlayer.Data;
using SQLite;
using System.Collections.Generic;

namespace OnePlayer.Data.Sqlite
{
    sealed class GenreTable : IGenreAccessor
    {
        private readonly SQLiteConnection connection;

        public GenreTable(SQLiteConnection connection)
        {
            this.connection = connection;
        }

        public Genre Add(Genre genre)
        {          
            connection.Insert(genre);
            return genre;
        }

        public void EnsureCreated()
        {
            connection.CreateTable<Genre>();
        }

        public Genre Find(string genreName)
        {
            string genreNameLower = genreName.ToLower();
            return connection.Table<Genre>().Where(genre => genre.NameLower == genreNameLower).FirstOrDefault();
        }

        public Genre Get(int id)
        {
            return connection.Table<Genre>().Where(genre => genre.Id == id).FirstOrDefault();
        }

        public IList<Genre> GetAll()
        {
            return connection.Table<Genre>().OrderBy(genre => genre.Name).ToList();
        }

        public Genre Update(Genre genre)
        {
            connection.Update(genre);
            return genre;
        }
    }
}
