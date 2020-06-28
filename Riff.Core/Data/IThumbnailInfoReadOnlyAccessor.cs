using System.Collections.Generic;

namespace Riff.Data
{
    public interface IThumbnailInfoReadOnlyAccessor
    {
        ThumbnailInfo Get(long id);

        IList<ThumbnailInfo> GetUncached();
    }
}
