using OnePlayer.Data;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace OnePlayer.UWP
{
    public static class ThumbnailCacheExtensions
    {
        public static async Task<StorageFile> GetStorageFile(this ThumbnailCache cache, long id)
        {
            return await StorageFile.GetFileFromPathAsync(cache.GetPath(id));
        }
    }
}
