using Plugin.LocalNotification;
using Microsoft.Maui.Storage;
using DepremTakipAPP.Models;
using DepremTakipAPP.Services; // LocalDatabaseService için
using System.Diagnostics;
using Newtonsoft.Json;
using SQLite; // SQLite için

namespace DepremTakipAPP.Helpers
{
    public static class EarthquakeNotificationHelper
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private static readonly HashSet<string> _notifiedIds = new();

        // KANDİLLİ ADRESİ (AFAD bozuk olduğu için)
        private const string ApiUrl = "https://api.orhanaydogdu.com.tr/deprem/kandilli/live?limit=25";

        public static async Task CheckAndNotifyFromApiAsync()
        {
            Debug.WriteLine(">>> Helper (DB): Kontrol başladı...");
            try
            {
                // 1. Depremleri Çek
                var jsonData = await _httpClient.GetStringAsync(ApiUrl);
                if (string.IsNullOrWhiteSpace(jsonData)) return;

                var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(jsonData);
                if (apiResponse?.Result == null || !apiResponse.Result.Any()) return;

                // 2. Veritabanından Takip Edilen Şehirleri Oku
                List<FollowedCitySetting> followedCities = new List<FollowedCitySetting>();

                try
                {
                    string dbPath = LocalDatabaseService.DbPath;
                    using (var db = new SQLiteConnection(dbPath))
                    {
                        db.CreateTable<FollowedCitySetting>();
                        followedCities = db.Table<FollowedCitySetting>().ToList();
                    }
                }
                catch (Exception dbEx)
                {
                    Debug.WriteLine($"DB Okuma Hatası: {dbEx.Message}");
                    return;
                }

                // --- İŞTE BU LOGU BURAYA EKLİYORUZ ---
                if (followedCities != null && followedCities.Count > 0)
                {
                    Debug.WriteLine($"✅ DB OKUNDU: Şu an {followedCities.Count} şehir takip ediliyor.");
                    foreach (var c in followedCities)
                    {
                        Debug.WriteLine($"   -> Şehir: {c.CityName}, Min: {c.MinMagnitude}");
                    }
                }
                else
                {
                    Debug.WriteLine("⚠️ DB OKUNDU: Ama liste boş.");
                }
                // --------------------------------------

                if (followedCities == null || followedCities.Count == 0) return;

                // 8.12.2025 /13.00 eklenen kontrol: 15 dakikadan eski depremleri atla
                foreach (var eq in apiResponse.Result)
                {
                    if (string.IsNullOrEmpty(eq.EarthquakeId) || string.IsNullOrEmpty(eq.Title)) continue;
                    if (_notifiedIds.Contains(eq.EarthquakeId)) continue;

                    if(!string.IsNullOrEmpty(eq.Date))
                    {
                        if (DateTime.TryParse(eq.Date, out DateTime eqDate))
                            {
                            var fark = DateTime.Now - eqDate;
                            if(fark.TotalMinutes > 15)
                            {
                                _notifiedIds.Add(eq.EarthquakeId);
                                // 24 saatten eski deprem, atla
                                continue;
                            }
                        }
                    }

                    foreach (var city in followedCities)
                    {
                        if (eq.Title.Contains(city.CityName, StringComparison.OrdinalIgnoreCase) &&
                            eq.Magnitude >= city.MinMagnitude)
                        {
                            await SendNotificationAsync(city.CityName, eq.Magnitude, eq.Date);
                            _notifiedIds.Add(eq.EarthquakeId);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"!!! Helper Hatası: {ex.Message}");
            }
        }

        private static async Task SendNotificationAsync(string cityName, double magnitude, string? date)
        {
            // Bildirim izni kontrolü (Arka planda bazen false dönebilir, try-catch ile sarmak iyi olur)
            try
            {
                var request = new NotificationRequest
                {
                    NotificationId = new Random().Next(1000, 9999),
                    Title = $"⚠️ Deprem Uyarısı: {cityName}",
                    Description = $"{date} - Büyüklük: {magnitude:F1}",
                    CategoryType = NotificationCategoryType.Alarm, // Alarm sesi çalsın
                    Schedule = new NotificationRequestSchedule { NotifyTime = DateTime.Now.AddSeconds(1) }
                };
                await LocalNotificationCenter.Current.Show(request);
            }
            catch { }
        }
    }
}