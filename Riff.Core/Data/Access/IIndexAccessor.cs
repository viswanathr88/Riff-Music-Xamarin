using Riff.Data.Access;
using System.Collections.Generic;

namespace Riff.Data.Access
{
    public interface IIndexAccessor : IIndexReadOnlyAccessor
    {
        IndexedTrack Add(IndexedTrack track);

        IndexedTrack Update(IndexedTrack track);

    }
}
