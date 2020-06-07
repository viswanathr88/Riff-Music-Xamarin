using Riff.Data;

namespace Riff.UWP.Storage
{
    public enum Theme
    {
        Dark,
        Light,
        Default
    };

    public interface IAppPreferences : IPreferences
    {
        Theme AppTheme { get; set; }
    }
}
