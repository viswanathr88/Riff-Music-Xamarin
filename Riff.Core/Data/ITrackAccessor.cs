using System.Collections.Generic;

namespace Riff.Data
{
    public interface ITrackAccessor : ITrackReadOnlyAccessor
    {
        Track Add(Track track);

        Track Update(Track track);
    }
}
