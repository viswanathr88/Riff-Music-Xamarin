using Riff.Data;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Riff.UWP.ViewModel
{
    public sealed class PlaylistViewModel : DataViewModel<Playlist>
    {
        private readonly IPlayer player;
        private ObservableCollection<DriveItem> items;

        public PlaylistViewModel(IPlayer player)
        {
            this.player = player;
        }

        public ObservableCollection<DriveItem> Items
        {
            get => items;
            private set => SetProperty(ref this.items, value);
        }

        public override async Task LoadAsync(Playlist playlist)
        {
            await playlist.LoadAsync();
            Items = new ObservableCollection<DriveItem>(playlist.Items);
        }

        public async Task Save()
        {
            await Parameter.SaveAsync();
        }
    }
}
