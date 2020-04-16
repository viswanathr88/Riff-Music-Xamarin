using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Views.Animations;
using Android.Widget;
using OnePlayer.Droid.UI.Auth;

namespace OnePlayer.Droid.UI
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.Fre")]
    public class FREActivity : AppCompatActivity
    {
        private TextView appName = null;

        // Pre login controls
        private TextView freLoginDescription = null;
        private Button freLoginButton = null;
        private ProgressBar freProgress = null;

        // Post login controls
        private ProgressBar frePostloginProgressbar = null;
        private ImageView frePostloginPhoto = null;
        private TextView frePostloginName = null;
        private TextView frePostloginEmail = null;
        private Button freNextButton = null;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.activity_fre);

            var appLogo = FindViewById<ImageView>(Resource.Id.fre_logo);
            appName = FindViewById<TextView>(Resource.Id.fre_app_name);

            // Pre login controls
            this.freProgress = FindViewById<ProgressBar>(Resource.Id.fre_spinner);
            this.freLoginDescription = FindViewById<TextView>(Resource.Id.fre_login_description);
            freLoginButton = FindViewById<Button>(Resource.Id.fre_login_button);
            freLoginButton.Click += LoginButton_Click;

            // Post login controls
            this.frePostloginProgressbar = FindViewById<ProgressBar>(Resource.Id.fre_postlogin_progressbar);
            this.frePostloginPhoto = FindViewById<ImageView>(Resource.Id.fre_postlogin_photo);
            this.frePostloginName = FindViewById<TextView>(Resource.Id.fre_postlogin_name);
            this.frePostloginEmail = FindViewById<TextView>(Resource.Id.fre_postlogin_email);
            this.freNextButton = FindViewById<Button>(Resource.Id.fre_next_button);
            freNextButton.Click += FreNextButton_Click;
        }

        private void FreNextButton_Click(object sender, System.EventArgs e)
        {
            var button = sender as Button;
            button.Click -= FreNextButton_Click;

            // Start the main activity
            StartActivity(new Android.Content.Intent(ApplicationContext, typeof(MainActivity)));
        }

        protected async override void OnResume()
        {
            base.OnResume();
            
            Animation slideUp = AnimationUtils.LoadAnimation(this, Resource.Animation.slide_up);
            appName.Visibility = Android.Views.ViewStates.Visible;
            appName.StartAnimation(slideUp);

            var app = (ApplicationContext as IOnePlayerApp);
            bool loginExists = await app.LoginManager.LoginExistsAsync();


            if (loginExists)
            {
                // Hide pre login related controls as we have an account
                freProgress.Visibility = Android.Views.ViewStates.Invisible;
                freLoginDescription.Visibility = Android.Views.ViewStates.Invisible;
                freLoginButton.Visibility = Android.Views.ViewStates.Invisible;

                // Post login
                frePostloginProgressbar.Visibility = Android.Views.ViewStates.Visible;
                frePostloginProgressbar.StartAnimation(slideUp);

                var profile = await app.LoginManager.GetUserAsync();
                using var photo = await app.LoginManager.GetUserPhotoAsync();

                frePostloginProgressbar.Visibility = Android.Views.ViewStates.Invisible;

                frePostloginName.Text = profile.DisplayName;
                frePostloginName.Visibility = Android.Views.ViewStates.Visible;
                frePostloginName.StartAnimation(slideUp);

                frePostloginEmail.Text = profile.Email;
                frePostloginEmail.Visibility = Android.Views.ViewStates.Visible;
                frePostloginEmail.StartAnimation(slideUp);


                frePostloginPhoto.SetImageBitmap(Android.Graphics.BitmapFactory.DecodeStream(photo));
                frePostloginPhoto.Visibility = Android.Views.ViewStates.Visible;
                frePostloginPhoto.StartAnimation(slideUp);

                freNextButton.Visibility = Android.Views.ViewStates.Visible;
                freNextButton.StartAnimation(slideUp);
            }
            else
            {
                freLoginDescription.Visibility = Android.Views.ViewStates.Visible;
                freLoginDescription.StartAnimation(slideUp);

                freLoginButton.Visibility = Android.Views.ViewStates.Visible;
                freLoginButton.StartAnimation(slideUp);

                freProgress.Visibility = Android.Views.ViewStates.Invisible;
                freLoginButton.Enabled = true;
            }
        }

        private void LoginButton_Click(object sender, System.EventArgs e)
        {
            freProgress.Visibility = Android.Views.ViewStates.Visible;
            freLoginButton.Enabled = false;
            StartActivity(new Android.Content.Intent(ApplicationContext, typeof(SignInAcitivity)));
        }

    }
}