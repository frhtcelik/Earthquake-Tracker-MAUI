using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using DepremTakipAPP.Services;
using System.Diagnostics;
using DepremTakipAPP.Models;
// Database servisini kullanabilmek için bu namespace'i ekliyoruz:
using DepremTakipAPP.Services;

namespace DepremTakipAPP
{
    public partial class MapPage : ContentPage
    {
        private readonly KandilliApiService _apiService;
        private readonly LocalDatabaseService _dbService; // Veritabanı servisi

        public MapPage()
        {
            InitializeComponent();

            // Servisleri başlatıyoruz
            _apiService = new KandilliApiService();
            _dbService = new LocalDatabaseService();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadAndDisplayEarthquakesOnMap();
        }

        private async Task LoadAndDisplayEarthquakesOnMap()
        {
            try
            {
                // 1. Haritayı Temizle
                EarthquakeMap.Pins.Clear();

                // 2. Haritayı Türkiye'ye Odakla
                var mapCenter = new Location(39.0, 35.0);
                EarthquakeMap.MoveToRegion(MapSpan.FromCenterAndRadius(mapCenter, Distance.FromKilometers(800)));

                // 3. Verileri Çek
                var earthquakes = await _apiService.GetLatestEarthquakesAsync();

                // --- 🔴 TEST İÇİN BURAYI EKLE (Test bitince silersin) ---
                // Sanki şu an Ağrı'da 7.5 büyüklüğünde deprem olmuş gibi listeye ekliyoruz.
                if (earthquakes != null)
                {
                    earthquakes.Add(new Earthquake
                    {
                        Title = "Ağrı - Merkez (TEST DEPREMİ)",
                        Magnitude = 7.5,
                        Depth = 5.0,
                        Latitude = 39.7191, // Ağrı Koordinatları
                        Longitude = 43.0503,
                        Date = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss")
                    });
                }
                // -------------------------------------------------------

                var followedCities = await _dbService.GetCitiesAsync();

                if (earthquakes != null && earthquakes.Any())
                {
                    foreach (var earthquake in earthquakes)
                    {
                        // 4. Bu deprem bizim takip ettiğimiz şehirlerden birinde mi?
                        bool isFollowed = false;

                        if (followedCities != null)
                        {
                            foreach (var city in followedCities)
                            {
                                // Şehir adı eşleşiyor mu VE Büyüklük kriteri tutuyor mu?
                                if (earthquake.Title.Contains(city.CityName, StringComparison.OrdinalIgnoreCase) &&
                                    earthquake.Magnitude >= city.MinMagnitude)
                                {
                                    isFollowed = true;
                                    break;
                                }
                            }
                        }

                        // 5. Pin Oluşturma
                        var pin = new Pin
                        {
                            Location = new Location(earthquake.Latitude, earthquake.Longitude),
                            Type = isFollowed ? PinType.SavedPin : PinType.Place, // Takip edilenler farklı görünür
                            Label = isFollowed ? $"⚠️ UYARI: {earthquake.Title}" : earthquake.Title,
                            Address = $"Büyüklük: {earthquake.Magnitude:F1} | {earthquake.Date}"
                        };

                        pin.InfoWindowClicked += async (sender, e) =>
                        {
                            var p = sender as Pin;
                            if (p != null)
                            {
                                await DisplayAlert("Deprem Detayı",
                                    $"{p.Label}\n\n{p.Address}\n\nLokasyon: {p.Location.Latitude}, {p.Location.Longitude}",
                                    "Kapat");
                            }
                        };

                        EarthquakeMap.Pins.Add(pin);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Harita Hatası: {ex.Message}");
            }
        }
    }
}