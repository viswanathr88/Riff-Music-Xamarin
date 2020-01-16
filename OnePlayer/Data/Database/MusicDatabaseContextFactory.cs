using OnePlayer.Data;
using System;
using System.IO;

namespace OnePlayer.Database
{
    sealed class MusicDatabaseContextFactory : IMusicDataContextFactory
    {
        private readonly string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "oneplayer.db");

        public MusicDatabaseContextFactory() { }

        public MusicDatabaseContextFactory(string dbPath)
        {
            if (string.IsNullOrEmpty(dbPath))
            {
                throw new ArgumentNullException(nameof(dbPath));
            }

            this.dbPath = dbPath;
        }
        public IMusicDataContext Create()
        {
            return new MusicDatabaseContext(dbPath);
        }
    }
}
