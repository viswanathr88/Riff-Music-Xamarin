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

        public bool Exists(int id, ThumbnailSize size)
        {
            string thumbnailPath = Path.Combine(rootPath, id.ToString(), $"{size.ToString()}.jpg");
            return File.Exists(thumbnailPath);
        }

        public Stream Get(int id, ThumbnailSize size)
        {
            string thumbnailPath = Path.Combine(rootPath, id.ToString(), $"{size.ToString()}.jpg");
            return new FileStream(thumbnailPath, FileMode.Open, FileAccess.Read);
        }

        public async Task SaveAsync(int id, Stream stream, ThumbnailSize size)
        {
            string thumbnailFolderPath = Path.Combine(rootPath, id.ToString());

            if (!Directory.Exists(thumbnailFolderPath))
            {
                Directory.CreateDirectory(thumbnailFolderPath);
            }

            string fullPath = Path.Combine(thumbnailFolderPath, $"{size.ToString()}.jpg");
            using (FileStream fstream = new FileStream(fullPath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                await stream.CopyToAsync(fstream);
            }
        }
    }
}
