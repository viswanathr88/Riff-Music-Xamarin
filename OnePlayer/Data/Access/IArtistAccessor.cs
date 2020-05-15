using System;
using System.Collections.Generic;
using System.Text;

namespace OnePlayer.Data.Access
{
    public interface IArtistAccessor : IArtistReadOnlyAccessor
    {
        Artist Add(Artist artist);

        Artist Update(Artist artist);
    }
}
