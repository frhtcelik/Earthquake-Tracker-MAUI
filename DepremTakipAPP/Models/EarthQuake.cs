using Newtonsoft.Json;

namespace DepremTakipAPP.Models
{
    public class Earthquake
    {
        public double DistanceToUser { get; set; } // Kullanıcının konumuna olan uzaklık (km cinsinden)
        public string DistanceText => DistanceToUser > 0 ? $"{Math.Round(DistanceToUser, 1)} km" : "-";

        [JsonProperty("earthquake_id")]
        public string? EarthquakeId { get; set; }

        [JsonProperty("title")]
        public string? Title { get; set; }

        // DÜZELTME BURADA YAPILDI: "date" yerine "date_time"
        [JsonProperty("date_time")]
        public string? Date { get; set; }

        [JsonProperty("mag")]
        public double Magnitude { get; set; }

        [JsonProperty("depth")]
        public double Depth { get; set; }

        [JsonProperty("lat")]
        public double Latitude { get; set; }

        [JsonProperty("lng")]
        public double Longitude { get; set; }

        // Ekranda sadece saati göstermek istersen bunu kullanırsın
        public string DisplayTime
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Date)) return "Saat Yok";

                try
                {
                    // Tarih "2025-12-03 19:19:40" şeklinde geliyor.
                    // Boşluktan bölüp saati alıyoruz.
                    var parts = Date.Split(' ');
                    if (parts.Length > 1) return parts[1];
                }
                catch { }

                return Date;
            }
        }
    }
}