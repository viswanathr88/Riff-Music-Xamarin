using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Riff.UWP.ViewModel
{
    class PlaylistsViewModel : DataViewModel
    {
        public override Task LoadAsync()
        {
            return Task.CompletedTask;
        }
    }
}
