using Riff.Data;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Riff.UWP.ViewModel
{
    public interface IMediaList : INotifyPropertyChanged
    {
        int CurrentIndex { get; set; }
        IMediaListItem CurrentItem { get; }
        Task AddItems(IList<DriveItem> items, uint startIndex);
        int Count { get; }
        IMediaListItem this[int index] { get; }
    }
}
