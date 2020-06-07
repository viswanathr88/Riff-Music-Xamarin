using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Riff.Sync;
using System.Threading.Tasks;

namespace Riff.Droid.Sync
{
    [Service(Name ="com.riff.droid.onedrivesync.service")]
    public sealed class OneDriveSyncService : Service, IOneDriveSyncService
    {
        private readonly NotificationCompat.Builder builder;
        public OneDriveSyncService()
        {
            builder = new NotificationCompat.Builder(this, "sync_notification_channel_id");
            builder.SetSmallIcon(Resource.Drawable.ic_search_artist);
        }

        public override IBinder OnBind(Intent intent)
        {
            return new OneDriveSyncServiceBinder(this);
        }

        public async Task SyncChanges()
        {
            var app = ApplicationContext as IRiffApp;

            app.SyncEngine.Checkpoint += Engine_Checkpoint;

            await app.SyncEngine.RunAsync();

            app.SyncEngine.Checkpoint -= Engine_Checkpoint;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        private void Engine_Checkpoint(object sender, SyncStatus e)
        {
            if (e.State == SyncState.Syncing && e.ItemsAdded == 0)
            {
                builder
                    .SetContentTitle("Looking for new music")
                    .SetProgress(0, 0, true);
            }
            else if (e.State == SyncState.Syncing)
            {
                builder
                    .SetContentText(e.ItemsAdded == 1 ? $"Added 1 item to library" : $"Added {e.ItemsAdded} items to library");
            }
            else if (e.State == SyncState.Uptodate)
            {
                if (e.ItemsAdded == 0)
                {
                    builder
                        .SetContentTitle("Sync completed")
                        .SetProgress(100, 100, false)
                        .SetContentText("Library up to date");
                }
                else
                {
                    builder
                        .SetContentTitle("Sync completed")
                        .SetProgress(100, 100, false)
                        .SetContentText(e.ItemsAdded == 1 ? $"Added 1 item to library" : $"Added {e.ItemsAdded} items to library");
                }
            }
            else
            {
                builder
                        .SetContentTitle("Sync failed")
                        .SetProgress(100, 100, false)
                        .SetContentText("Please try again later");
            }

            NotifyNotification();
        }

        private void NotifyNotification()
        {
            var notificationManager = NotificationManagerCompat.From(this);
            notificationManager.Notify((int)UI.NotificationType.SyncProgress, builder.Build());
        }
    }
}