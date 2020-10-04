using Mirage.ViewModel.Commands;
using Riff.Sync;
using System;
using System.Threading.Tasks;

namespace Riff.UWP.ViewModel.Commands
{
    public sealed class SyncCommand : Command
    {
        private readonly SyncEngine syncEngine;

        public SyncCommand(SyncEngine engine)
        {
            this.syncEngine = engine ?? throw new ArgumentNullException(nameof(syncEngine));
            this.syncEngine.StateChanged += SyncEngine_StateChanged;
        }

        protected override bool CanExecute()
        {
            return syncEngine.State != SyncState.NotStarted && syncEngine.State != SyncState.Syncing && syncEngine.State != SyncState.Stopped;
        }

        protected override void Run()
        {
            Task.Run(() => syncEngine.RunAsync());
        }

        private async void SyncEngine_StateChanged(object sender, SyncState e) => await UIHelper.RunUISafe(() => EvaluateCanExecute());
    }
}
