using System;
using System.IO;
using System.Threading.Tasks;

namespace OnePlayer.Data
{
    sealed class ThumbnailCache : IThumbnailCache
    {
        private readonly string rootPath;
        private readonly string relativePath;

        public ThumbnailCache(string path, string relativePath)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            rootPath = path;
            this.relativePath = relativePath;
            
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }
        }

        public bool Exists(int id, ThumbnailSize size)
        {
            return File.Exists(GetPath(id, size));
        }

        public Stream Get(int id, ThumbnailSize size)
        {
            return new FileStream(GetPath(id, size), FileMode.Open, FileAccess.Read);
        }

        public string GetPath(int id, ThumbnailSize size)
        {
            return Path.Combine(rootPath, id.ToString(), $"{size}.jpg");
        }

        public string GetRelativePath(int id, ThumbnailSize size)
        {
            return Path.Combine(relativePath, id.ToString(), $"{size}.jpg");
        }

        public async Task SaveAsync(int id, Stream stream, ThumbnailSize size)
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
