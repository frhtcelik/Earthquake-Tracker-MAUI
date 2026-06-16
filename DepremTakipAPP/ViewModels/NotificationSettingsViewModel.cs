using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using Plugin.LocalNotification;
using DepremTakipAPP.Models;
using DepremTakipAPP.Services;
using Microsoft.Maui.ApplicationModel.Communication;
using System.Text.Json;

namespace DepremTakipAPP.ViewModels
{
    public class NotificationSettingsViewModel : INotifyPropertyChanged
    {
        private readonly LocalDatabaseService _dbService;

        public ObservableCollection<string> AllCities { get; }

        private string _selectedCityFromPicker;
        public string SelectedCityFromPicker
        {
            get => _selectedCityFromPicker;
            set { _selectedCityFromPicker = value; OnPropertyChanged(nameof(SelectedCityFromPicker)); }
        }

        private double _minMagnitude = 4.0;
        public double MinMagnitude
        {
            get => _minMagnitude;
            set
            {
                _minMagnitude = Math.Round(value * 2) / 2.0;
                OnPropertyChanged(nameof(MinMagnitude));
                OnPropertyChanged(nameof(MinMagnitudeLabel));
            }
        }

        public string MinMagnitudeLabel => $"Minimum Büyüklük: {MinMagnitude:F1}";
        public ObservableCollection<FollowedCitySetting> FollowedCities { get; set; }

        public ICommand AddCityCommand { get; }
        public ICommand RemoveCityCommand { get; }
        public ICommand SendTestNotificationCommand { get; }

        // --- GÜVENDEYİM / ACİL DURUM MENÜSÜ AYARLARI ---

        private bool _isMenuOpen = false;
        public bool IsMenuOpen
        {
            get => _isMenuOpen;
            set { _isMenuOpen = value; OnPropertyChanged(nameof(IsMenuOpen)); }
        }

        public ObservableCollection<AcilKisi> AcilKisiler { get; set; }

        public ICommand ToggleMenuCommand { get; }
        public ICommand KisiSecCommand { get; }
        public ICommand RemoveKisiCommand { get; }
        public ICommand GuvendeyimCommand { get; }
        public ICommand GuvendeDegilimCommand { get; } // SADECE BU TANIM EKLENDİ

        public NotificationSettingsViewModel()
        {
            _dbService = new LocalDatabaseService();

            AllCities = new ObservableCollection<string>
            {
                "İstanbul", "Ankara", "İzmir", "Bursa", "Antalya", "Adana", "Konya", "Gaziantep", "Şanlıurfa", "Kocaeli", "Mersin", "Diyarbakır", "Hatay", "Manisa", "Kayseri", "Samsun", "Balıkesir", "Kahramanmaraş", "Van", "Aydın", "Tekirdağ", "Sakarya", "Muğla", "Eskişehir", "Trabzon", "Mardin", "Malatya", "Erzurum", "Ordu", "Afyonkarahisar", "Sivas", "Adıyaman", "Tokat", "Zonguldak", "Elazığ", "Batman", "Kütahya", "Çanakkale", "Osmaniye", "Çorum", "Şırnak", "Giresun", "Isparta", "Aksaray", "Yozgat", "Muş", "Düzce", "Edirne", "Kastamonu", "Uşak", "Niğde", "Kırklareli", "Bitlis", "Rize", "Amasya", "Siirt", "Bolu", "Nevşehir", "Kars", "Hakkari", "Kırıkkale", "Bingöl", "Burdur", "Karaman", "Karabük", "Yalova", "Kırşehir", "Erzincan", "Bilecik", "Sinop", "Iğdır", "Bartın", "Çankırı", "Artvin", "Gümüşhane", "Kilis", "Ardahan", "Tunceli", "Bayburt"
            };

            FollowedCities = new ObservableCollection<FollowedCitySetting>();
            AcilKisiler = new ObservableCollection<AcilKisi>();

            AddCityCommand = new Command(async () => await AddSelectedCityAsync());
            RemoveCityCommand = new Command<FollowedCitySetting>(async (city) => await RemoveCityAsync(city));
            SendTestNotificationCommand = new Command(async () => await SendTestNotificationAsync());

            ToggleMenuCommand = new Command(() => IsMenuOpen = !IsMenuOpen);
            KisiSecCommand = new Command(async () => await KisiSecAsync());
            GuvendeyimCommand = new Command(async () => await SendGuvendeyimMessageAsync());
            GuvendeDegilimCommand = new Command(async () => await SendGuvendeDegilimMessageAsync()); // SADECE BU BAĞLANTI EKLENDİ

            RemoveKisiCommand = new Command<AcilKisi>(kisi =>
            {
                if (kisi != null && AcilKisiler.Contains(kisi))
                {
                    AcilKisiler.Remove(kisi);
                    KisileriHafizayaKaydet();
                }
            });

            Task.Run(async () => await LoadSettingsAsync());
            HafizadanKisileriYukle();
        }

        private void KisileriHafizayaKaydet()
        {
            var json = JsonSerializer.Serialize(AcilKisiler.ToList());
            Preferences.Default.Set("AcilKisilerListesi", json);
        }

        private void HafizadanKisileriYukle()
        {
            var json = Preferences.Default.Get("AcilKisilerListesi", string.Empty);
            if (!string.IsNullOrEmpty(json))
            {
                var liste = JsonSerializer.Deserialize<List<AcilKisi>>(json);
                if (liste != null)
                {
                    foreach (var kisi in liste)
                        AcilKisiler.Add(kisi);
                }
            }
        }

        private async Task KisiSecAsync()
        {
            try
            {
                if (!MainThread.IsMainThread)
                {
                    await MainThread.InvokeOnMainThreadAsync(KisiSecAsync);
                    return;
                }

                if (AcilKisiler.Count >= 3)
                {
                    await App.Current.MainPage.DisplayAlert("Sınır", "En fazla 3 kişi ekleyebilirsiniz.", "Tamam");
                    return;
                }

                var status = await Permissions.CheckStatusAsync<Permissions.ContactsRead>();
                if (status != PermissionStatus.Granted)
                    status = await Permissions.RequestAsync<Permissions.ContactsRead>();

                if (status != PermissionStatus.Granted)
                {
                    await App.Current.MainPage.DisplayAlert("Uyarı", "Rehbere erişim izni gerekiyor.", "Tamam");
                    return;
                }

                var contact = await Microsoft.Maui.ApplicationModel.Communication.Contacts.Default.PickContactAsync();

                if (contact != null && contact.Phones.Count > 0)
                {
                    string isim = contact.DisplayName ?? "Kayıtlı Kişi";
                    string numara = contact.Phones[0].PhoneNumber.Replace(" ", "");

                    if (AcilKisiler.Any(k => k.Numara == numara))
                    {
                        await App.Current.MainPage.DisplayAlert("Uyarı", "Bu numara zaten ekli.", "Tamam");
                        return;
                    }

                    AcilKisiler.Add(new AcilKisi { Isim = isim, Numara = numara });
                    KisileriHafizayaKaydet();
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Hata", $"Rehber açılamadı: {ex.Message}", "Tamam");
            }
        }

        private async Task SendGuvendeyimMessageAsync()
        {
            if (AcilKisiler.Count == 0)
            {
                await App.Current.MainPage.DisplayAlert("Bilgi", "Lütfen önce kişi ekleyin.", "Tamam");
                return;
            }

            string messageText = "Ben güvendeyim, durumum iyi. Lütfen merak etmeyin.";

            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.Sms>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.Sms>();
                }

                if (status != PermissionStatus.Granted)
                {
                    await App.Current.MainPage.DisplayAlert("Hata", "Arka planda SMS gönderebilmek için izin vermeniz gerekiyor.", "Tamam");
                    return;
                }

#if ANDROID
                var smsManager = Android.Telephony.SmsManager.Default;
                
                foreach (var kisi in AcilKisiler)
                {
                    smsManager.SendTextMessage(kisi.Numara, null, messageText, null, null);
                }

                await App.Current.MainPage.DisplayAlert("Başarılı", "Güvendeyim mesajı arka planda otomatik olarak iletildi.", "Tamam");
                IsMenuOpen = false;
#else
                var numaralar = AcilKisiler.Select(k => k.Numara).ToArray();
                var smsMessage = new SmsMessage(messageText, numaralar);
                await Sms.Default.ComposeAsync(smsMessage);
                IsMenuOpen = false;
#endif
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Hata", $"Mesaj gönderilemedi: {ex.Message}", "Tamam");
            }
        }

        // SADECE BU YENİ YARDIM ÇAĞRISI METODU EN ALTA EKLENDİ
        private async Task SendGuvendeDegilimMessageAsync()
        {
            if (AcilKisiler.Count == 0)
            {
                await App.Current.MainPage.DisplayAlert("Bilgi", "Lütfen önce kişi ekleyin.", "Tamam");
                return;
            }

            string messageText = "ACİL DURUM! Güvende değilim, acil yardıma ihtiyacım var.";

            try
            {
                // 1. SMS İZNİ
                var smsStatus = await Permissions.CheckStatusAsync<Permissions.Sms>();
                if (smsStatus != PermissionStatus.Granted)
                    smsStatus = await Permissions.RequestAsync<Permissions.Sms>();

                if (smsStatus != PermissionStatus.Granted)
                {
                    await App.Current.MainPage.DisplayAlert("Hata", "SMS izni verilmedi.", "Tamam");
                    return;
                }

                // 2. KONUM İZNİ
                var locationStatus = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (locationStatus != PermissionStatus.Granted)
                    locationStatus = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

                if (locationStatus == PermissionStatus.Granted)
                {
                    try
                    {
                        var location = await Geolocation.Default.GetLastKnownLocationAsync();

                        if (location == null)
                        {
                            var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));
                            location = await Geolocation.Default.GetLocationAsync(request);
                        }

                        if (location != null)
                        {
                            string lat = location.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
                            string lon = location.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
                            messageText += $"\nKonumum: https://maps.google.com/?q={lat},{lon}";
                        }
                    }
                    catch (FeatureNotEnabledException)
                    {
                        messageText += "\n(Cihazın Konum/GPS özelliği kapalı olduğu için koordinat alınamadı!)";
                    }
                    catch (Exception)
                    {
                        messageText += "\n(Konum şu an uydudan çekilemedi!)";
                    }
                }
                else
                {
                    messageText += "\n(Konum izni reddedildiği için eklenemedi!)";
                }

#if ANDROID
                var smsManager = Android.Telephony.SmsManager.Default;
                foreach (var kisi in AcilKisiler)
                {
                    smsManager.SendTextMessage(kisi.Numara, null, messageText, null, null);
                }
                await App.Current.MainPage.DisplayAlert("Yardım Çağrısı", "Mesajınız ve konumunuz iletildi!", "Tamam");
                IsMenuOpen = false;
#else
                var numaralar = AcilKisiler.Select(k => k.Numara).ToArray();
                var smsMessage = new SmsMessage(messageText, numaralar);
                await Sms.Default.ComposeAsync(smsMessage);
                IsMenuOpen = false;
#endif
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Hata", $"Gönderim başarısız: {ex.Message}", "Tamam");
            }
        }

        // --- MEVCUT METOTLAR ---
        private async Task AddSelectedCityAsync()
        {
            if (string.IsNullOrWhiteSpace(SelectedCityFromPicker)) return;
            var exists = FollowedCities.Any(c => c.CityName.Equals(SelectedCityFromPicker, StringComparison.OrdinalIgnoreCase));
            if (exists) return;

            var newSetting = new FollowedCitySetting { CityName = SelectedCityFromPicker, MinMagnitude = this.MinMagnitude };
            try { await _dbService.AddCityAsync(newSetting); FollowedCities.Add(newSetting); } catch { }
        }

        private async Task RemoveCityAsync(FollowedCitySetting cityToRemove)
        {
            if (cityToRemove != null) { await _dbService.RemoveCityAsync(cityToRemove); FollowedCities.Remove(cityToRemove); }
        }

        private async Task LoadSettingsAsync()
        {
            var dbList = await _dbService.GetCitiesAsync();
            MainThread.BeginInvokeOnMainThread(() =>
            {
                FollowedCities.Clear();
                foreach (var city in dbList) FollowedCities.Add(city);
            });
        }

        private async Task SendTestNotificationAsync()
        {
            if (await LocalNotificationCenter.Current.RequestNotificationPermission() == false) return;
            var request = new NotificationRequest { NotificationId = 9999, Title = "Test", Description = "Başarılı.", Schedule = new NotificationRequestSchedule { NotifyTime = DateTime.Now.AddSeconds(1) } };
            await LocalNotificationCenter.Current.Show(request);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class AcilKisi
    {
        public string Isim { get; set; }
        public string Numara { get; set; }
        public string Gorunum => $"{Isim}";
    }
}