using OnePlayer.Sync;
using System;
using System.Threading.Tasks;

namespace OnePlayer.UWP.ViewModel
{
    public sealed class SyncViewModel : DataViewModel
    {
        private readonly SyncEngine engine;

        public SyncViewModel(SyncEngine engine)
        {
            this.engine = engine ?? throw new ArgumentNullException(nameof(engine));
        }

        public override Task LoadAsync()
        {
            IsLoaded = true;
            return Task.CompletedTask;
        }
    }
}
