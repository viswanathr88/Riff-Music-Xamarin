using System;
using System.IO;
using System.Threading.Tasks;

namespace OnePlayer.Data
{
    public sealed class ThumbnailCache
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

        public bool Exists(long id)
        {
            return File.Exists(GetPath(id));
        }

        public Stream Get(long id)
        {
            return new FileStream(GetPath(id), FileMode.Open, FileAccess.Read);
        }

        public string GetPath(long id)
        {
            return Path.Combine(rootPath, $"{id}.jpg");
        }

        public async Task SaveAsync(long id, Stream stream)
        {
            using (FileStream fstream = new FileStream(GetPath(id), FileMode.OpenOrCreate, FileAccess.Write))
            {
                await stream.CopyToAsync(fstream);
            }
        }
    }
}
