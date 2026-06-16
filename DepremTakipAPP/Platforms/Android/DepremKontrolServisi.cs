using Android.App;
using Android.Content;
using Android.OS;
using DepremTakipAPP.Services;
// Artık System.Diagnostics.Debug diyeceğimiz için using'i kaldırabiliriz veya bırakabiliriz.

namespace DepremTakipAPP.Platforms.Android
{
    [Service]
    public class DepremKontrolServisi : Service
    {
        public override IBinder OnBind(Intent intent) => null;

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            // "Debug" çakışmasını önlemek için tam adını yazıyoruz.
            System.Diagnostics.Debug.WriteLine(">>> Android Servisi: Deprem kontrolü başlıyor...");

            Task.Run(async () =>
            {
                try
                {
                    var checkService = new EarthquakeCheckService();
                    await checkService.CheckForEarthquakesAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"!!!!!! DepremKontrolServisi HATA: {ex.Message}");
                }
                finally
                {
                    StopSelf();
                }
            });

            return StartCommandResult.Sticky;
        }

        public override void OnDestroy()
        {
            System.Diagnostics.Debug.WriteLine(">>> Android Servisi: Durduruldu.");
            base.OnDestroy();
        }
    }
}