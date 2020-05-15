using System.Collections.Generic;

namespace OnePlayer.Data.Access
{
    public interface IThumbnailInfoReadOnlyAccessor
    {
        ThumbnailInfo Get(long id);

        IList<ThumbnailInfo> GetUncached();
    }
}
