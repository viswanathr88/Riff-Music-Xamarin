using Riff.UWP.Pages;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Riff.UWP.Test.UI
{
    public class AlbumsPageTest : IDisposable
    {
        public AlbumsPageTest()
        {
        }

        [Fact]
        public async Task AlbumsPage_Navigate()
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => App.RootFrame.Navigate(typeof(AlbumsPage), null));
            
            await Task.Delay(20000);
        }
        public void Dispose()
        {
        }
    }
}
