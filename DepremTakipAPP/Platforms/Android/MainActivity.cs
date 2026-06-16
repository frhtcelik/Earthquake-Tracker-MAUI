using Android.App;
using Android.Content; // Intent için bu satır gerekli
using Android.Content.PM;
using Android.OS; // Build.VERSION için bu satır gerekli
using DepremTakipAPP.Platforms.Android; // MyBackgroundTask servisini bulabilmek için bu satır gerekli

namespace DepremTakipAPP
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Arka planda çalışacak servisi başlatmak için bir "niyet" (Intent) oluştur
            var serviceIntent = new Intent(this, typeof(MyBackgroundTask));

            // Android 8.0 (Oreo) ve daha yeni sürümler,
            // arka plan servisleri için farklı bir başlatma yöntemi gerektirir.
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                StartForegroundService(serviceIntent);
            }
            else
            {
                StartService(serviceIntent);
            }
        }
    }
}