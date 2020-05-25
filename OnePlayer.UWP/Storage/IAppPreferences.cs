using OnePlayer.Data;

namespace OnePlayer.UWP.Storage
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
