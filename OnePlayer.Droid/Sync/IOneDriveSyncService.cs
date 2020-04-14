using System.Threading.Tasks;

namespace OnePlayer.Droid.Sync
{
    public interface IOneDriveSyncService
    {
        Task SyncChanges();
    }
}