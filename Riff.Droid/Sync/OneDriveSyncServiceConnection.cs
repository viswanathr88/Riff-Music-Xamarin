using Android.Content;
using Android.OS;
using System.Threading.Tasks;

namespace Riff.Droid.Sync
{
    sealed class OneDriveSyncServiceConnection : Java.Lang.Object, IServiceConnection
    {
        public bool IsConnected { get; private set;  }
        public async void OnServiceConnected(ComponentName name, IBinder service)
        {
            IsConnected = service != null;

            if (IsConnected)
            {
                var binder = service as OneDriveSyncServiceBinder;
                await Task.Run(async() => await binder.SyncService.SyncChanges());
            }
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            IsConnected = false;
        }
    }
}