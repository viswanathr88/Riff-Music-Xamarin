using SQLite;
using System;
using System.Collections.Generic;

namespace OnePlayer.Data.Sqlite
{
    sealed class ThumbnailInfoTable : IThumbnailInfoAccessor
    {
        private readonly SQLiteConnection connection;

        public ThumbnailInfoTable(SQLiteConnection connection)
        {
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public ThumbnailInfo Add(ThumbnailInfo info)
        {
            this.connection.Insert(info);
            return info;
        }

        public void EnsureCreated()
        {
            this.connection.CreateTable<ThumbnailInfo>();
        }

        public ThumbnailInfo Get(int id)
        {
            return this.connection.Table<ThumbnailInfo>().Where(info => info.Id == id).FirstOrDefault();
        }

        public IList<ThumbnailInfo> GetAll()
        {
            return this.connection.Table<ThumbnailInfo>().ToList();
        }

        public IList<ThumbnailInfo> GetUncached(int? limit)
        {
            return this.connection.Table<ThumbnailInfo>().Where(info => !info.Cached).ToList();
        }

        public ThumbnailInfo Update(ThumbnailInfo info)
        {
            this.connection.Update(info);
            return info;
        }
    }
}
