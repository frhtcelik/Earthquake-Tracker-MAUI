using SQLite;

namespace DepremTakipAPP.Models
{
    [Table("FollowedCities")]
    public class FollowedCitySetting
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string CityName { get; set; }
        public double MinMagnitude { get; set; }

        public override string ToString()
        {
            return $"{CityName} (Min: {MinMagnitude:F1})";
        }
    }
}