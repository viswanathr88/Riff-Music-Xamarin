using OnePlayer.Data;
using SQLite;
using System;
using System.Collections.Generic;

namespace OnePlayer.Database
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
            if (genre.Id.HasValue)
            {
                throw new Exception("Id already exists");
            }
            
            connection.Insert(genre);
            return genre;
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
            return connection.Table<Genre>().ToList();
        }

        public Genre Update(Genre genre)
        {
            if (!genre.Id.HasValue)
            {
                throw new Exception("Id cannot be null");
            }

            connection.Update(genre);
            return genre;
        }
    }
}
