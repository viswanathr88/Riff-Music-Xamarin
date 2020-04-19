using OnePlayer.Data;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace OnePlayer.UWP.ViewModel
{
    public sealed class MusicLibraryViewModel : DataViewModel<VoidType>
    {
        private ObservableCollection<Album> albums = null;

        public MusicLibraryViewModel()
        {
            Albums = new ObservableCollection<Album>();
        }

        public ObservableCollection<Album> Albums
        {
            get
            {
                return this.albums;
            }
            private set
            {
                SetProperty(ref this.albums, value);
            }
        }

        public override Task LoadAsync(VoidType parameter)
        {
            for (int i = 0; i < 200; i++)
            {
                Albums.Add(new Album()
                {
                    Name = "Test Album " + (i + 1),
                    ReleaseYear = 2000 + i,
                });
            }

            return Task.CompletedTask;
        }
    }
}
