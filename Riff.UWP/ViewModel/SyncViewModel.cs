using Riff.Data;
using Riff.Sync;
using Riff.UWP.ViewModel.Commands;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Riff.UWP.ViewModel
{
    public sealed class SyncViewModel : DataViewModel
    {
        private readonly SyncEngine syncEngine;
        private readonly SyncCommand syncCommand;
        private readonly IPreferences preferences;
        private SyncState state;

        public SyncViewModel(SyncEngine engine, IPreferences preferences)
        {
            this.syncEngine = engine ?? throw new ArgumentNullException(nameof(engine));
            this.syncEngine.StateChanged += SyncEngine_StateChanged;
            this.preferences = preferences ?? throw new ArgumentNullException(nameof(preferences));
            this.syncCommand = new SyncCommand(this.syncEngine);
            State = this.syncEngine.State;
        }

        public SyncState State
        {
            get
            {
                return this.state;
            }
            private set
            {
                SetProperty(ref this.state, value);
            }
        }

        public bool IsSyncPaused
        {
            get => this.preferences.IsSyncPaused;
            private set
            {
                if (IsSyncPaused != value)
                {
                    preferences.IsSyncPaused = value;
                    RaisePropertyChanged(nameof(IsSyncPaused));
                }
            }
        }

        public override Task LoadAsync()
        {
            Load();
            IsLoaded = true;
            return Task.CompletedTask;
        }

        public void Load()
        {
            if (this.syncCommand.CanExecute(null))
            {
                this.syncCommand.Execute(null);
            }
        }

        public ICommand SyncNow => syncCommand;

        public void PauseSync()
        {
            if (!IsSyncPaused)
            {
                syncEngine.Stop();
                RaisePropertyChanged(nameof(IsSyncPaused));
            }
        }

        public void ResumeSync()
        {
            if (IsSyncPaused)
            {
                IsSyncPaused = false;
                syncEngine.Start();
                syncCommand.Execute(null);
            }
        }

        private async void SyncEngine_StateChanged(object sender, SyncState e)
        {
            await RunUISafe(() => State = e);
        }
    }
}
