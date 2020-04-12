using SQLite;
using System;
using System.Collections.Generic;

namespace OnePlayer.Data.Sqlite
{
    sealed class AlbumTable : IAlbumAccessor
    {
        private readonly SQLiteConnection connection;

        public AlbumTable(SQLiteConnection connection)
        {
            this.connection = connection;
        }

        public Album Add(Album album)
        {
            connection.Insert(album);
            return album;
        }

        public void EnsureCreated()
        {
            connection.CreateTable<Album>();
        }

        public Album FindByArtist(int artistId, string albumName)
        {
            string albumNameLower = albumName.ToLower();
            return connection.Table<Album>().Where(album => album.ArtistId == artistId && album.NameLower == albumNameLower).FirstOrDefault();
        }

        public Album Get(int id)
        {
            return connection.Table<Album>().Where(album => album.Id == id).FirstOrDefault();
        }

        public IList<Album> GetAll()
        {
            return GetAll(AlbumSortType.ReleaseYear, SortOrder.Descending);
        }

        public IList<Album> GetAll(AlbumSortType type, SortOrder order)
        {
            var query = connection.Table<Album>();
            if (type == AlbumSortType.ReleaseYear)
            {
                query = (order == SortOrder.Ascending) ? query.OrderBy(album => album.ReleaseYear) : query.OrderByDescending(album => album.ReleaseYear);
            }
            else if (type == AlbumSortType.Title)
            {
                query = (order == SortOrder.Ascending) ? query.OrderBy(album => album.Name) : query.OrderByDescending(album => album.Name);
            }

            return query.ToList();
        }

        public IList<Album> GetAll(AlbumSortType type, SortOrder order, int startPosition, int count)
        {
            throw new NotImplementedException();
        }

        public IList<Album> GetAllByArtist(int artistId)
        {
            return connection.Table<Album>().Where(album => album.ArtistId == artistId).ToList();
        }

        public int GetCount()
        {
            return connection.Table<Album>().Count();
        }

        public Album Update(Album album)
        {
            connection.Update(album);
            return album;
        }
    }
}
