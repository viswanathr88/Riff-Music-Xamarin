using Riff.Sync;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace Riff.UWP.ViewModel.Commands
{
    public sealed class SyncCommand : ICommand
    {
        private readonly SyncEngine syncEngine;

        public SyncCommand(SyncEngine engine)
        {
            this.syncEngine = engine ?? throw new ArgumentNullException(nameof(syncEngine));
            this.syncEngine.StateChanged += SyncEngine_StateChanged;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return syncEngine.State != SyncState.NotStarted || syncEngine.State != SyncState.Syncing;
        }

        public void Execute(object parameter)
        {
            Task.Run(() => syncEngine.RunAsync());
        }

        private async void SyncEngine_StateChanged(object sender, SyncState e) => await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => CanExecuteChanged?.Invoke(this, EventArgs.Empty));
    }
}
