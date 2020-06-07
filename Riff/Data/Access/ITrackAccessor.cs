using System.Collections.Generic;

namespace Riff.Data.Access
{
    public interface ITrackAccessor : ITrackReadOnlyAccessor
    {
        Track Add(Track track);

        Track Update(Track track);
    }
}
