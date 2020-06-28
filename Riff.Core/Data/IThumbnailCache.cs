using System.IO;
using System.Threading.Tasks;

namespace Riff.Data
{
    public interface IThumbnailReadOnlyCache
    {
        bool Exists(long id);
        Stream Get(long id);
        string GetPath(long id);
    }

    public interface IThumbnailCache : IThumbnailReadOnlyCache
    {
        Task SaveAsync(long id, Stream stream);
    }
}