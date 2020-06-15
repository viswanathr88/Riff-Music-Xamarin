using System;
using System.Threading.Tasks;

namespace Riff.Sync
{
    public interface ITrackUrlDownloader
    {
        Task<Uri> GetDownloadUrlAsync(string driveItemId);
    }
}