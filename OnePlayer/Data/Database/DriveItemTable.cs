using OnePlayer.Data;
using SQLite;
using System;

namespace OnePlayer.Database
{
    sealed class DriveItemTable : IDriveItemAccessor
    {
        private readonly SQLiteConnection connection;

        public DriveItemTable(SQLiteConnection connection)
        {
            this.connection = connection;
        }

        public DriveItem Add(DriveItem item)
        {
            connection.InsertOrReplace(item);
            return item;
        }

        public DriveItem Get(string id)
        {
            return connection.Table<DriveItem>().Where(item => item.Id == id).FirstOrDefault();
        }

        public DriveItem Update(DriveItem item)
        {
            if (string.IsNullOrEmpty(item.Id))
            {
                throw new Exception("Id is null or empty");
            }

            connection.Update(item);
            return item;
        }
    }
}
