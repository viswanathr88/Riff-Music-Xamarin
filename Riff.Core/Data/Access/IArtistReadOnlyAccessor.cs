using System.Collections.Generic;

namespace Riff.Data.Access
{
    public interface IArtistReadOnlyAccessor
    {
        Artist Get(long id);

        Artist Find(string artistName);

        IList<Artist> GetAll();
    }
}
