using Riff.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Riff.UWP.ViewModel
{
    public interface IMediaList
    {
        int CurrentIndex { get; set; }
        IMediaListItem CurrentItem { get; }
        Task SetItems(IList<DriveItem> items, uint startIndex);
        int Count { get; }
        IMediaListItem this[int index] { get; }
    }
}
