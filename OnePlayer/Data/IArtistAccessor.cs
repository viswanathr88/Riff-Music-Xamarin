using System.Collections.Generic;

namespace OnePlayer.Data
{
    public interface IArtistAccessor
    {
        void EnsureCreated();

        Artist Get(int id);

        Artist Find(string artistName);

        IList<Artist> GetAll();

        Artist Add(Artist artist);

        Artist Update(Artist artist);
    }
}
