using OnePlayer.Data.Access;
using System.Collections.Generic;

namespace OnePlayer.Data.Access
{
    public interface IIndexAccessor : IIndexReadOnlyAccessor
    {
        IndexedTrack Add(IndexedTrack track);

        IndexedTrack Update(IndexedTrack track);

    }
}
