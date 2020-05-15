using Microsoft.Data.Sqlite;
using System;

namespace OnePlayer.Data.Sqlite
{
    static class SqliteExtensions
    {
        public static SqliteParameter AddWithNullableValue(this SqliteParameterCollection collection, string key, object value)
        {
            if (value == null)
            {
                return collection.AddWithValue(key, DBNull.Value);
            }
            else
            {
                return collection.AddWithValue(key, value);
            }
        }
    }
}
