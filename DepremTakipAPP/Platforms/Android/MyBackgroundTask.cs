using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using DepremTakipAPP.Helpers;
using System.Diagnostics;
using Android.Content.PM; 

// Namespace'in bu olduğundan emin ol (dosya Platforms/Android altında olmalı)
namespace DepremTakipAPP.Platforms.Android
{
    // HATA DÜZELTMESİ (1/2):
    // Servise, Android 14+ için GEREKLİ OLAN "ForegroundServiceType"
    // özelliğini doğrudan C# attribute'u üzerinden veriyoruz.
    [Service(ForegroundServiceType = ForegroundService.TypeDataSync)]
    public class MyBackgroundTask : Service
    {
        private CancellationTokenSource? _cts;
        private bool _isRunning = false;
        private const int SERVICE_NOTIFICATION_ID = 10001;
        private const string CHANNEL_ID = "DepremServisKanali";

        public override IBinder? OnBind(Intent? intent) => null;

        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
                return;

            var channelName = "Deprem Takip Servisi";
            var channelDescription = "Arka planda depremleri kontrol eden servis.";
            var channel = new NotificationChannel(CHANNEL_ID, channelName, NotificationImportance.Low);

            var notificationManager = GetSystemService(NotificationService) as NotificationManager;
            notificationManager?.CreateNotificationChannel(channel);
        }

        public override StartCommandResult OnStartCommand(Intent? intent, StartCommandFlags flags, int startId)
        {
            if (_isRunning)
            {
                System.Diagnostics.Debug.WriteLine(">>> MyBackgroundTask: Servis zaten çalışıyor.");
                return StartCommandResult.Sticky;
            }

            _cts = new CancellationTokenSource();
            _isRunning = true;
            System.Diagnostics.Debug.WriteLine(">>> MyBackgroundTask: Servis başlatıldı.");

            CreateNotificationChannel();

            var notification = new NotificationCompat.Builder(this, CHANNEL_ID)
                .SetContentTitle("Deprem Takip Uygulaması")
                .SetContentText("Arka planda deprem kontrolü yapılıyor...")
                .SetSmallIcon(Resource.Mipmap.appicon)
                .SetOngoing(true)
                .Build();

            // Bu, Android'e verdiğimiz sözü tutar.
            StartForeground(SERVICE_NOTIFICATION_ID, notification);

            Task.Run(async () =>
            {
                try
                {
                    while (!_cts.Token.IsCancellationRequested)
                    {
                        System.Diagnostics.Debug.WriteLine(">>> MyBackgroundTask: Deprem kontrolü (Helper) başlıyor...");
                       
                        await EarthquakeNotificationHelper.CheckAndNotifyFromApiAsync();
                        System.Diagnostics.Debug.WriteLine(">>> MyBackgroundTask: Kontrol bitti, 5 dakika bekleniyor.");

                        await Task.Delay(TimeSpan.FromMinutes(5), _cts.Token);
                    }
                }
                catch (System.OperationCanceledException) { /* ... */ }
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"!!! MyBackgroundTask HATA: {ex.Message}"); }
                finally
                {
                    _isRunning = false;
                    System.Diagnostics.Debug.WriteLine(">>> MyBackgroundTask: Görev döngüsü durdu.");
                }
            }, _cts.Token);

            return StartCommandResult.Sticky;
        }

        public override void OnDestroy()
        {
            _cts?.Cancel();
            _isRunning = false;
            StopForeground(true);
            System.Diagnostics.Debug.WriteLine(">>> MyBackgroundTask: Servis yok ediliyor.");
            base.OnDestroy();
        }
    }
}