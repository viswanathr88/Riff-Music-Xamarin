namespace Riff.UWP.Pages
{
    public interface IShellPage
    {
        bool CanGoBack { get; }

        void GoBack();
    }
}
