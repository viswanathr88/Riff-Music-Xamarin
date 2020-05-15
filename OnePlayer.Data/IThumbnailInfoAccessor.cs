using System.Collections.Generic;

namespace OnePlayer.Data
{
    public interface IThumbnailInfoAccessor
    {
        ThumbnailInfo Get(long id);

        ThumbnailInfo Add(ThumbnailInfo info);

        ThumbnailInfo Update(ThumbnailInfo info);

        IList<ThumbnailInfo> GetUncached();
    }
}
