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
        bool Exists(int id, ThumbnailSize size);
        Stream Get(int id, ThumbnailSize size);
        Task SaveAsync(int id, Stream stream, ThumbnailSize size);
    }
}