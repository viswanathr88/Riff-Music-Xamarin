using OnePlayer.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlayer.UWP.ViewModel
{
    public sealed class HomeViewModel : DataViewModel
    {
        private readonly MusicLibrary library;
        private ObservableCollection<DriveItem> recentlyAddedItems;

        public HomeViewModel(MusicLibrary library)
        {
            this.library = library ?? throw new ArgumentNullException(nameof(library));
        }

        public ObservableCollection<DriveItem> RecentlyAddedItems
        {
            get => recentlyAddedItems;
            private set => SetProperty(ref recentlyAddedItems, value);
        }

        public override Task LoadAsync(VoidType parameter)
        {
            throw new NotImplementedException();
        }
    }
}
