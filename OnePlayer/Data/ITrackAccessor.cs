using System;
using System.Collections.Generic;
using System.Text;

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
