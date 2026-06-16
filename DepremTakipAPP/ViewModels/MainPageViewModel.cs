using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using DepremTakipAPP.Models;
using DepremTakipAPP.Services;
using Microsoft.Maui.Devices.Sensors; // Mesafe hesaplama ve GPS için gerekli

namespace DepremTakipAPP.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private readonly KandilliApiService _apiService = new KandilliApiService();

        public ObservableCollection<Earthquake> Earthquakes { get; set; } = new ObservableCollection<Earthquake>();

        private bool _isLoading = false;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        public MainPageViewModel()
        {
        }

        public async Task LoadEarthquakesAsync()
        {
            if (IsLoading) return;

            IsLoading = true;
            Earthquakes.Clear();

            // 1. Önce kullanıcının anlık konumunu alıyoruz
            Location userLocation = null;
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(5));
                userLocation = await Geolocation.Default.GetLocationAsync(request);
            }
            catch (Exception ex)
            {
                // GPS kapalıysa veya izin yoksa hata vermemesi için boş geçiyoruz
                System.Diagnostics.Debug.WriteLine($"Konum alınamadı: {ex.Message}");
            }

            // 2. API'den deprem listesini çekiyoruz
            var earthquakeList = await _apiService.GetLatestEarthquakesAsync();

            if (earthquakeList != null)
            {
                foreach (var earthquake in earthquakeList)
                {
                    // 3. Eğer konum alındıysa mesafeyi hesapla
                    if (userLocation != null)
                    {
                        // Depremin koordinatlarını alıyoruz
                        Location earthquakeLocation = new Location(earthquake.Latitude, earthquake.Longitude);

                        // İki nokta arasındaki mesafeyi KM olarak hesaplıyoruz
                        double distance = Location.CalculateDistance(userLocation, earthquakeLocation, DistanceUnits.Kilometers);

                        // Hesaplanan mesafeyi modele kaydediyoruz
                        earthquake.DistanceToUser = distance;
                    }

                    Earthquakes.Add(earthquake);
                }
            }

            IsLoading = false;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}