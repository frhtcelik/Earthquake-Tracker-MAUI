using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using DepremTakipAPP.Services;
using DepremTakipAPP.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DepremTakipAPP
{
    public partial class HeatmapPage : ContentPage
    {
        private readonly KandilliApiService _apiService;

        // SUNUM MODU ANAHTARI: 
        // true yaparsan hoca için hazýrlanan devasa depremler görünür.
        // false yaparsan internetten anlýk gerçek veriyi çeker.
        private bool _sunumModuAcik = true;

        public HeatmapPage()
        {
            InitializeComponent();
            _apiService = new KandilliApiService();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await CizimleriOlusturAsync();
        }

        private async Task CizimleriOlusturAsync()
        {
            // Kullanýlacak veriyi tutacak liste (Interface olarak tanýmladýk ki iki farklý listeyi de kabul etsin)
            IEnumerable<Earthquake> gosterilecekDepremler;

            if (_sunumModuAcik)
            {
                // 1. DURUM: SUNUM ÝÇÝN ÖZEL HAZIRLANMIÞ LÝSTE
                gosterilecekDepremler = new List<Earthquake>
                {
                    // Güneydoðu (Devasa hareketlilik)
                    new Earthquake { Latitude = 37.2858, Longitude = 36.9371, Magnitude = 7.8 },
                    new Earthquake { Latitude = 38.0818, Longitude = 37.1773, Magnitude = 7.6 },
                    new Earthquake { Latitude = 37.5000, Longitude = 37.0000, Magnitude = 6.4 },
                    new Earthquake { Latitude = 37.8000, Longitude = 36.8000, Magnitude = 5.8 },

                    // Ege 
                    new Earthquake { Latitude = 37.8881, Longitude = 26.7770, Magnitude = 6.6 },
                    new Earthquake { Latitude = 37.9500, Longitude = 26.8500, Magnitude = 4.5 },

                    // Marmara
                    new Earthquake { Latitude = 40.8433, Longitude = 28.1400, Magnitude = 5.2 },
                    new Earthquake { Latitude = 40.8000, Longitude = 28.5000, Magnitude = 4.1 },

                    // Ýç Anadolu arka plan detaylarý
                    new Earthquake { Latitude = 39.1122, Longitude = 35.1234, Magnitude = 3.5 },
                    new Earthquake { Latitude = 39.4234, Longitude = 35.3456, Magnitude = 2.8 },
                    new Earthquake { Latitude = 38.9000, Longitude = 35.5000, Magnitude = 3.1 }
                };
            }
            else
            {
                // 2. DURUM: SENÝN ORÝJÝNAL ÇALIÞAN KODUN (CANLI API)
                gosterilecekDepremler = await _apiService.GetLatestEarthquakesAsync();

                // API'den veri gelmezse iþlemi durdur
                if (gosterilecekDepremler == null) return;
            }

            // Haritadaki eski çizimleri temizle
            HeatmapMap.MapElements.Clear();

            // SENÝN ÇÝZÝM ALGORÝTMAN (BÝREBÝR AYNI)
            foreach (var deprem in gosterilecekDepremler)
            {
                Color cemberRengi;
                Distance yariCap;

                if (deprem.Magnitude >= 5.0)
                {
                    cemberRengi = Colors.DarkRed.WithAlpha(0.6f);
                    yariCap = Distance.FromKilometers(deprem.Magnitude * 8);
                }
                else if (deprem.Magnitude >= 4.0)
                {
                    cemberRengi = Colors.OrangeRed.WithAlpha(0.5f);
                    yariCap = Distance.FromKilometers(deprem.Magnitude * 5);
                }
                else if (deprem.Magnitude >= 3.0)
                {
                    cemberRengi = Colors.Orange.WithAlpha(0.4f);
                    yariCap = Distance.FromKilometers(deprem.Magnitude * 3);
                }
                else
                {
                    cemberRengi = Colors.Yellow.WithAlpha(0.3f);
                    yariCap = Distance.FromKilometers(deprem.Magnitude * 1.5);
                }

                var circle = new Microsoft.Maui.Controls.Maps.Circle
                {
                    Center = new Location(deprem.Latitude, deprem.Longitude),
                    Radius = yariCap,
                    StrokeColor = cemberRengi,
                    StrokeWidth = 0,
                    FillColor = cemberRengi
                };

                HeatmapMap.MapElements.Add(circle);
            }

            // Haritayý tüm Türkiye'yi görecek þekilde konumlandýr
            var turkiyeMerkez = new Location(38.9637, 35.2433);
            var haritaAcisi = MapSpan.FromCenterAndRadius(turkiyeMerkez, Distance.FromKilometers(800));
            HeatmapMap.MoveToRegion(haritaAcisi);
        }
    }
}