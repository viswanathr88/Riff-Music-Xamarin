using Mirage.ViewModel.Commands;
using Riff.Data;
using System.Collections.Generic;

namespace Riff.UWP.ViewModel
{
    public interface ITrackCommands
    {
        IAsyncCommand<IList<DriveItem>> Play { get; }

        IAsyncCommand<DriveItem> PlayNext { get; }
    }
}
