using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnePlayer.UWP.Pages
{
    public interface ISupportViewModel<T>
    {
        T ViewModel { get; }
    }
}
