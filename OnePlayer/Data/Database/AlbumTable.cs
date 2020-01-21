using OnePlayer.Data;
using SQLite;
using System;
using System.Collections.Generic;

namespace OnePlayer.Database
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
            return connection.Table<Album>().ToList();
        }

        public IList<Album> GetAllByArtist(int artistId)
        {
            return connection.Table<Album>().Where(album => album.ArtistId == artistId).ToList();
        }

        public Album Update(Album album)
        {
            connection.Update(album);
            return album;
        }
    }
}
