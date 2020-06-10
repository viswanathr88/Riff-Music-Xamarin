namespace Riff.UWP.Pages
{
    public abstract class ShellPageBase : PageBase
    {
        public abstract bool CanGoBack { get; }

        public abstract void GoBack();
    }
}
