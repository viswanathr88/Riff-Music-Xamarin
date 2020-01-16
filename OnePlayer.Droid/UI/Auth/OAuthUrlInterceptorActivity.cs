
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

namespace OnePlayer.Droid.UI.Auth
{
    [Activity(Label = nameof(OAuthUrlInterceptorActivity), NoHistory = true, LaunchMode = LaunchMode.SingleTop)]
    [IntentFilter(
    new[] { Intent.ActionView },
    Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
    DataSchemes = new[] { "app" },
    DataHost = "com.oneplayer.droid",
    DataPath = "/oauth2redirect")]
    public class OAuthUrlInterceptorActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Load redirectUrl page
            Intent intent = new Intent(Intent.ActionView, Intent.Data, this, typeof(SignInAcitivity));
            intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);
            StartActivity(intent);

            Finish();
        }
    }
}