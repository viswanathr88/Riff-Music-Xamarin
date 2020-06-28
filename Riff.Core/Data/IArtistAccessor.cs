using System;
using System.Collections.Generic;
using System.Text;

namespace Riff.Data
{
    public interface IArtistAccessor : IArtistReadOnlyAccessor
    {
        Artist Add(Artist artist);

        Artist Update(Artist artist);
    }
}
