using DepremTakipAPP.Models;

namespace DepremTakipAPP;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Senin mevcut kaydettiğin rotalar (Dokunulmadı)
        Routing.RegisterRoute("DepremListesi", typeof(Earthquake));
        Routing.RegisterRoute("HaritaSayfasi", typeof(MapPage));

        // YENİ ROTALAR: Eskiden alt menüde olan sayfaları artık navigasyonla açabilmek için kaydediyoruz.
        Routing.RegisterRoute("MapPage", typeof(MapPage));
        Routing.RegisterRoute("HeatmapPage", typeof(HeatmapPage));
        Routing.RegisterRoute("StatisticsPage", typeof(StatisticsPage));
        Routing.RegisterRoute("MainPage", typeof(MainPage));
        Routing.RegisterRoute("AyarlarSayfasi", typeof(NotificationSettingsPage));
    }
}