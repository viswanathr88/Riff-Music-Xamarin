using Riff.Data;
using Riff.Data.Sqlite;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.StartScreen;

namespace Riff.UWP.ViewModel
{
    public sealed class PlaylistViewModel : DataViewModel<Playlist>
    {
        private readonly IMusicLibrary musicLibrary;
        private ObservableCollection<PlaylistItem> items;
        private IList<DriveItem> tracks;
        private int? oldIndex = null;

        public PlaylistViewModel(IMusicLibrary musicLibrary)
        {
            this.musicLibrary = musicLibrary;
        }

        public ObservableCollection<PlaylistItem> Items
        {
            get => items;
            private set
            {
                if (items != null)
                {
                    items.CollectionChanged -= PlaylistItems_CollectionChanged;
                }
                
                if (SetProperty(ref this.items, value))
                {
                    value.CollectionChanged += PlaylistItems_CollectionChanged;
                }
            }
        }

        private void PlaylistItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    oldIndex = e.OldStartingIndex;
                    break;
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    {
                        if (oldIndex.HasValue)
                        {
                            using (var session = musicLibrary.Edit())
                            {
                                session.PlaylistItems.Reorder(Parameter, e.NewStartingIndex, oldIndex.Value, 1);
                                session.Save();
                            }

                            oldIndex = null;
                        }
                        break;
                    }
            }
        }

        public IList<DriveItem> Tracks
        {
            get => tracks;
            private set
            {
                SetProperty(ref this.tracks, value);
            }
        }

        public override async Task LoadAsync(Playlist playlist)
        {
            var items = await Task.Run(() =>
            {
                var options = new PlaylistItemAccessOptions()
                {
                    PlaylistFilter = playlist.Id,
                    IncludeDriveItem = true
                };
                return musicLibrary.PlaylistItems.Get(options);
            });
            Items = new ObservableCollection<PlaylistItem>(items);
            Tracks = items.Select(playlistItem => playlistItem.DriveItem).ToList();
        }
    }
}
