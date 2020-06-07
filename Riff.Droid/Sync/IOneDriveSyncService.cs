using System.Threading.Tasks;

namespace Riff.Droid.Sync
{
    public interface IOneDriveSyncService
    {
        Task SyncChanges();
    }
}