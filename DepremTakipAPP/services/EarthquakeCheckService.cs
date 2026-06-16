using DepremTakipAPP.Helpers; // Bu using ifadesi önemli
using System.Diagnostics;
// Artık bu servisin diğer using'lere ihtiyacı yok
// (Json, Storage, ViewModels vb. hepsi Helper'da)

namespace DepremTakipAPP.Services
{
    public class EarthquakeCheckService
    {
        // Bu servisin artık KandilliApiService'e ihtiyacı yok,
        // çünkü Helper bu işi zaten yapıyor.
        // private readonly KandilliApiService _apiService = new();

        public async Task CheckForEarthquakesAsync()
        {
            Debug.WriteLine(">>> Arka plan servisi (CheckService): Başladı. Helper'ı çağıracak...");

            //
            // HATA DÜZELTMESİ BURADA:
            // Artık parametre göndermiyoruz. Sadece Helper'daki metodu çağırıyoruz.
            // Bu metot API'den veriyi çekecek ve filtreleyecektir.
            //
            await EarthquakeNotificationHelper.CheckAndNotifyFromApiAsync();

            Debug.WriteLine(">>> Arka plan servisi (CheckService): Görev tamamlandı.");
        }
    }
}