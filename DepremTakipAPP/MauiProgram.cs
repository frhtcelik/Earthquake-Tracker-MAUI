using DepremTakipAPP.Services;
using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;

#if ANDROID
using Microsoft.Maui.Controls.Handlers;
using Google.Android.Material.BottomNavigation;
#endif

namespace DepremTakipAPP
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            // Servislerimizi kaydediyoruz (bu kısım doğru)
            builder.Services.AddSingleton<KandilliApiService>();

            builder
                .UseMauiApp<App>()
                .UseLocalNotification()
                .UseMauiMaps()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            builder.Services.AddSingleton<KandilliApiService>();
            builder.Services.AddSingleton<LocalDatabaseService>();

#if DEBUG
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}