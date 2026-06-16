using System.Text.Json;
using Microsoft.Maui.Storage;
using DepremTakipAPP.ViewModels;
using DepremTakipAPP.Models;

namespace DepremTakipAPP
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();

            var testCities = new List<FollowedCitySetting>
    {
        new FollowedCitySetting { CityName = "İstanbul", MinMagnitude = 1.0 }
    };
            Preferences.Set("FollowedCitiesList", JsonSerializer.Serialize(testCities));
        }
    }
}