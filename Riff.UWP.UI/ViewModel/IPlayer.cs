using Riff.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Riff.UWP.ViewModel
{
    public interface IPlayer
    {
        event EventHandler<EventArgs> CurrentTrackChanged;

        IMediaList PlaybackList { get; }

        Task PlayAsync(IList<DriveItem> items, uint startIndex, bool autoplay = true);

        Task PlayAsync(Album album, bool autoplay = true);

        Task PlayAsync(Artist artist, bool autoplay = true);
    }
}
