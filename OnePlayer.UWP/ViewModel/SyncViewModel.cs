using OnePlayer.Sync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlayer.UWP.ViewModel
{
    class SyncViewModel : DataViewModel<VoidType>
    {
        private readonly SyncEngine engine;

        public SyncViewModel(SyncEngine engine)
        {
            this.engine = engine ?? throw new ArgumentNullException(nameof(engine));
        }

        public override Task LoadAsync(VoidType parameter)
        {
            IsLoaded = true;
            return Task.CompletedTask;
        }
    }
}
