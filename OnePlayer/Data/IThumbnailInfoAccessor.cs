using System.Collections.Generic;

namespace OnePlayer.Data
{
    public interface IThumbnailInfoAccessor
    {
        void EnsureCreated();

        ThumbnailInfo Get(int id);

        ThumbnailInfo Add(ThumbnailInfo info);

        ThumbnailInfo Update(ThumbnailInfo info);

        IList<ThumbnailInfo> GetAll();

        IList<ThumbnailInfo> GetUncached();
    }
}
