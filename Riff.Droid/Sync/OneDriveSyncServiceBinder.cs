using Android.OS;

namespace Riff.Droid.Sync
{
    sealed class OneDriveSyncServiceBinder : Binder
    {
        public OneDriveSyncServiceBinder(IOneDriveSyncService syncService)
        {
            SyncService = syncService;
        }

        public IOneDriveSyncService SyncService
        {
            get;
        }
    }
}