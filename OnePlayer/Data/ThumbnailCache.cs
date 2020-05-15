using System;
using System.IO;
using System.Threading.Tasks;

namespace OnePlayer.Data
{
    sealed class ThumbnailCache : IThumbnailCache
    {
        private readonly string rootPath;

        public ThumbnailCache(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            rootPath = path;
            
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }
        }

        public bool Exists(long id, ThumbnailSize size)
        {
            return File.Exists(GetPath(id, size));
        }

        public Stream Get(long id, ThumbnailSize size)
        {
            return new FileStream(GetPath(id, size), FileMode.Open, FileAccess.Read);
        }

        public string GetPath(long id, ThumbnailSize size)
        {
            return Path.Combine(rootPath, id.ToString(), $"{size}.jpg");
        }

        public async Task SaveAsync(long id, Stream stream, ThumbnailSize size)
        {
            string thumbnailFolderPath = Path.Combine(rootPath, id.ToString());

            if (!Directory.Exists(thumbnailFolderPath))
            {
                Directory.CreateDirectory(thumbnailFolderPath);
            }

            string fullPath = Path.Combine(thumbnailFolderPath, $"{size}.jpg");
            using (FileStream fstream = new FileStream(fullPath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                await stream.CopyToAsync(fstream);
            }
        }
    }
}
