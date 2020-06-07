using System.Collections.Generic;

namespace Riff.Data.Access
{
    public interface IThumbnailInfoReadOnlyAccessor
    {
        ThumbnailInfo Get(long id);

        IList<ThumbnailInfo> GetUncached();
    }
}
