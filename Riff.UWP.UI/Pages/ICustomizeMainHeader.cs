using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Media;

namespace Riff.UWP.Pages
{
    public interface ICustomizeMainHeader
    {
        string HeaderText { get; }

        Brush ShellBackground { get; }

        double HeaderOpacity { get; }
    }
}
