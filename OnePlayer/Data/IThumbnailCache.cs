using System.IO;
using System.Threading.Tasks;

namespace OnePlayer.Data
{
    public enum ThumbnailSize
    {
        Small,
        Medium
    };

    public interface IThumbnailCache
    {
        bool Exists(long id, ThumbnailSize size);
        Stream Get(long id, ThumbnailSize size);
        string GetPath(long id, ThumbnailSize size);
        Task SaveAsync(long id, Stream stream, ThumbnailSize size);
    }
}