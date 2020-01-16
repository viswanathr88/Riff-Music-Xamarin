using Android.App;
using Android.OS;
using Android.Support.CustomTabs;
using System;

namespace OnePlayer.Droid.UI.Auth
{
    [Activity(Label = "Sign in to OneDrive", Name = "com.oneplayer.droid.SignInActivity", LaunchMode = Android.Content.PM.LaunchMode.SingleTop)]
    public class SignInAcitivity : Activity
    {
        private IOnePlayerApp appContext = null;
        public SignInAcitivity()
        {
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.appContext = ApplicationContext as IOnePlayerApp;

            // Get URL
            var authorizeUrl = appContext.LoginManager.GetAuthorizeUrl();

            // Load URL in a browser
            CustomTabsIntent.Builder builder = new CustomTabsIntent.Builder();
            CustomTabsIntent customTabsIntent = builder.Build();
            customTabsIntent.LaunchUrl(this, Android.Net.Uri.Parse(authorizeUrl));
        }

        protected async override void OnNewIntent(Android.Content.Intent intent)
        {
            base.OnNewIntent(intent);
            string code = intent.Data.GetQueryParameter("code");

            try
            {
                await this.appContext.LoginManager.EndLoginAsync(code);
                Android.Widget.Toast.MakeText(ApplicationContext, "Successfully signed in to OneDrive", Android.Widget.ToastLength.Short).Show();
            }
            catch (Exception ex)
            {
                Android.Widget.Toast.MakeText(ApplicationContext, $"Failed to sign-in - {ex.Message}", Android.Widget.ToastLength.Short).Show();
            }

            Finish();
        }
    }
}