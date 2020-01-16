using Android.Content;

namespace OnePlayer.Droid.Storage
{
    sealed class AppPreferences : Data.IPreferences
    {
        private readonly ISharedPreferences sharedPreferences;
        private const string deltaUrlKey = "DeltaUrl";

        public AppPreferences(ISharedPreferences sharedPreferences)
        {
            this.sharedPreferences = sharedPreferences;
        }

        public string DeltaUrl
        {
            get => sharedPreferences.GetString(deltaUrlKey, null);
            set => sharedPreferences.Edit().PutString(deltaUrlKey, value).Apply();
        }
    }
}