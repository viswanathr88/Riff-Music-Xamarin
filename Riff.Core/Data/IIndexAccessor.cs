using System.Collections.Generic;

namespace Riff.Data
{
    public interface IIndexAccessor : IIndexReadOnlyAccessor
    {
        IndexedTrack Add(IndexedTrack track);

        IndexedTrack Update(IndexedTrack track);

    }
}
