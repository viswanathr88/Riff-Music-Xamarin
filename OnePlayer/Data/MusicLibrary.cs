using System;

namespace OnePlayer.Data
{
    public sealed class MusicLibrary
    {
        private readonly IMusicDataContextFactory dataContextFactory;
        public MusicLibrary()
        {
            dataContextFactory = new OnePlayer.Database.MusicDatabaseContextFactory();

            using (var context = dataContextFactory.Create())
            {
                context.Migrate();
            }
        }

        public MusicLibrary(IMusicDataContextFactory dataContextFactory)
        {
            this.dataContextFactory = dataContextFactory ?? throw new ArgumentNullException(nameof(dataContextFactory));
        }

        internal MusicLibraryWriter Edit()
        {
            return new MusicLibraryWriter(dataContextFactory);
        }

    }
}
