using Android.App;
using Android.Content;
using Android.OS;
using System.Threading.Tasks;

namespace Riff.Droid.UI
{
    [Activity(Theme = "@style/AppTheme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : Android.Support.V7.App.AppCompatActivity
    {
        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);
        }

        // Launches the startup task
        protected override void OnResume()
        {
            base.OnResume();
            Task startupWork = new Task(async () => { await StartupAsync(); });
            startupWork.Start();
        }

        private async Task StartupAsync()
        {
            IRiffApp app = ApplicationContext as IRiffApp;
            if (await app.LoginManager.LoginExistsAsync())
            {
                StartActivity(new Intent(Application.Context, typeof(MainActivity)));
            }
            else
            {
                // Adding the transition animation to the ActivityOptions and include them in a Bundle
                Bundle animationBundle = ActivityOptions.MakeCustomAnimation(this, Resource.Animation.abc_fade_in, Resource.Animation.abc_fade_out).ToBundle();
                StartActivity(new Intent(Application.Context, typeof(FREActivity)), animationBundle);
            }
        }
    }
}