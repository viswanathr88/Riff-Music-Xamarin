using System.Collections.Generic;

namespace OnePlayer.Data
{
    public interface ITrackAccessor
    {
        void EnsureCreated();

        Track Get(int id);

        IList<Track> GetAll();

        Track Add(Track track);

        Track Update(Track track);
    }
}
