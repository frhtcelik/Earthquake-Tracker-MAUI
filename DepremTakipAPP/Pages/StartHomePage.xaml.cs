using Microsoft.Maui.Controls;
using System;

namespace DepremTakipAPP.Pages
{
    public partial class StartHomePage : ContentPage
    {
        public StartHomePage()
        {
            InitializeComponent();
        }

        // 1. Son Depremler sayfasýna git
        private async void OnSonDepremlerTapped(object sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("MainPage");
        }

        // 2. Harita sayfasýna git
        private async void OnHaritaTapped(object sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("MapPage");
        }

        // 3. Isý Haritasý sayfasýna git
        private async void OnIsiHaritasiTapped(object sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("HeatmapPage");
        }

        // 4. Ýstatistikler sayfasýna git
        private async void OnIstatistikTapped(object sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("StatisticsPage");
        }

        // 5. Ayarlar ve Güvenlik sayfasýna git
        private async void OnAyarlarTapped(object sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("AyarlarSayfasi");
        }
    }
}