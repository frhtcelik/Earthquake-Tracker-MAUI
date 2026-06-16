using DepremTakipAPP.Models;
using DepremTakipAPP.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace DepremTakipAPP
{
    public partial class StatisticsPage : ContentPage, INotifyPropertyChanged
    {
        private readonly KandilliApiService _apiService;

        private string _toplamDepremText;
        public string ToplamDepremText { get => _toplamDepremText; set { _toplamDepremText = value; OnPropertyChanged(); } }

        private string _ortalamaSiddetText;
        public string OrtalamaSiddetText { get => _ortalamaSiddetText; set { _ortalamaSiddetText = value; OnPropertyChanged(); } }

        private string _enBuyukDepremText;
        public string EnBuyukDepremText { get => _enBuyukDepremText; set { _enBuyukDepremText = value; OnPropertyChanged(); } }

        private double _hafifOran;
        public double HafifOran { get => _hafifOran; set { _hafifOran = value; OnPropertyChanged(); } }

        private double _ortaOran;
        public double OrtaOran { get => _ortaOran; set { _ortaOran = value; OnPropertyChanged(); } }

        private double _siddetliOran;
        public double SiddetliOran { get => _siddetliOran; set { _siddetliOran = value; OnPropertyChanged(); } }

        public StatisticsPage()
        {
            InitializeComponent();
            BindingContext = this;
            _apiService = new KandilliApiService();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await IstatistikleriHesaplaAsync();
        }

        private async Task IstatistikleriHesaplaAsync()
        {
            // ÝÞLEM BAÞLIYOR: Animasyonu göster
            LoadingIndicator.IsRunning = true;
            LoadingIndicator.IsVisible = true;

            bool sunumModuAcik = true; // True yaparsan örnek veriler gelir
            IEnumerable<Earthquake> depremler;

            if (sunumModuAcik)
            {
                // Sunum verileri olduðu için veri çekme süresini simüle edelim (Yarým saniye beklet)
                await Task.Delay(500);

                depremler = new List<Earthquake>
                {
                    new Earthquake { Magnitude = 7.8, Title = "Pazarcýk (Kahramanmaraþ)" },
                    new Earthquake { Magnitude = 7.6, Title = "Elbistan (Kahramanmaraþ)" },
                    new Earthquake { Magnitude = 6.4, Title = "Defne (Hatay)" },
                    new Earthquake { Magnitude = 5.2, Title = "Marmara Denizi" },
                    new Earthquake { Magnitude = 4.1, Title = "Seferihisar (Ýzmir)" },
                    new Earthquake { Magnitude = 3.5, Title = "Sivas" },
                    new Earthquake { Magnitude = 2.8, Title = "Ankara" },
                    new Earthquake { Magnitude = 2.1, Title = "Bursa" },
                    new Earthquake { Magnitude = 1.8, Title = "Muðla" },
                    new Earthquake { Magnitude = 1.5, Title = "Antalya" }
                };
            }
            else
            {
                depremler = await _apiService.GetLatestEarthquakesAsync();
                if (depremler == null || !depremler.Any())
                {
                    // Hata olursa animasyonu kapat ve çýk
                    LoadingIndicator.IsRunning = false;
                    LoadingIndicator.IsVisible = false;
                    return;
                }
            }

            int toplam = depremler.Count();
            if (toplam == 0) return;

            double ortalama = depremler.Average(d => d.Magnitude);
            var enBuyuk = depremler.OrderByDescending(d => d.Magnitude).First();

            ToplamDepremText = toplam.ToString();
            OrtalamaSiddetText = Math.Round(ortalama, 1).ToString();
            EnBuyukDepremText = $"{enBuyuk.Magnitude} ({enBuyuk.Title})";

            double hafifSayisi = depremler.Count(d => d.Magnitude < 3.0);
            double ortaSayisi = depremler.Count(d => d.Magnitude >= 3.0 && d.Magnitude < 4.5);
            double siddetliSayisi = depremler.Count(d => d.Magnitude >= 4.5);

            HafifOran = hafifSayisi / toplam;
            OrtaOran = ortaSayisi / toplam;
            SiddetliOran = siddetliSayisi / toplam;

            // ÝÞLEM BÝTTÝ: Animasyonu gizle
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
        }

        public new event PropertyChangedEventHandler PropertyChanged;
        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}