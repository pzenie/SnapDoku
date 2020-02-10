using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;

namespace SnapDoku_Xamarin.Droid
{
    [Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true, ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashActivity : AppCompatActivity
    {
        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);
            SetContentView(Resource.Layout.SplashLayout);
        }

        // Launches the startup task
        protected override void OnResume()
        {
            base.OnResume();
            SetContentView(Resource.Layout.SplashLayout);
            Task startupWork = new Task(() => { SimulateStartup(); });
            startupWork.Start();
        }

        // Simulates background work that happens behind the splash screen
        void SimulateStartup()
        {
            Thread.Sleep(2000);
            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }
    }
}