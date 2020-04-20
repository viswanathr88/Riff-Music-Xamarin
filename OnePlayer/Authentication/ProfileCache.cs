using Newtonsoft.Json;
using OnePlayer.Data;
using System.IO;
using System.Threading.Tasks;

namespace OnePlayer.Authentication
{
    public class ProfileCache : IProfileCache
    {
        private readonly string path;
        private readonly static string folderName = "Profile";
        private readonly static string profileName = "OneDriveProfile.json";
        private readonly static string photoName = "OneDrivePhoto.jpg";

        public ProfileCache(string path)
        {
            this.path = System.IO.Path.Combine(path, folderName);
            if (!Directory.Exists(this.path))
            {
                Directory.CreateDirectory(this.path);
            }
        }

        public async Task<UserProfile> GetProfileAsync()
        {
            string path = System.IO.Path.Combine(this.path, profileName);
            UserProfile profile = null;
            if (File.Exists(path))
            {
                using (FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string json = await reader.ReadToEndAsync();
                        profile = JsonConvert.DeserializeObject<UserProfile>(json);
                    }
                }
            }

            return profile;
        }

        public Task SetProfileAsync(UserProfile profile)
        {
            string path = System.IO.Path.Combine(this.path, profileName);
            using (FileStream stream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(writer, profile);
                    return Task.CompletedTask;
                }
            }
        }

        public Task<Stream> GetPhotoAsync()
        {
            string path = System.IO.Path.Combine(this.path, photoName);
            if (File.Exists(path))
            {
                return Task.FromResult<Stream>(new FileStream(path, FileMode.Open, FileAccess.Read));
            }

            return Task.FromResult<Stream>(null);
        }

        public async Task SetPhotoAsync(Stream stream)
        {
            string path = System.IO.Path.Combine(this.path, photoName);
            using (FileStream fstream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
            {
                await stream.CopyToAsync(fstream);
            }
        }
    }
}