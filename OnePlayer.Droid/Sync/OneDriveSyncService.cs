using Android.App;
using Android.Content;
using Android.OS;
using OnePlayer.Sync;
using System.Threading.Tasks;

namespace OnePlayer.Droid.Sync
{
    [Service(Name ="com.oneplayer.droid.onedrivesync.service")]
    public sealed class OneDriveSyncService : Service, IOneDriveSyncService
    {
        public override IBinder OnBind(Intent intent)
        {
            return new OneDriveSyncServiceBinder(this);
        }

        public async Task SyncChanges()
        {
            var app = ApplicationContext as IOnePlayerApp;
            OneDriveMusicSyncJob job = new OneDriveMusicSyncJob(app.Preferences, app.LoginManager, app.WebClient, app.MusicLibrary);
            await job.RunAsync();
        }
    }
}