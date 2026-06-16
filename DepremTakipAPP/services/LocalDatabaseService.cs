using SQLite;
using DepremTakipAPP.Models;

namespace DepremTakipAPP.Services
{
    public class LocalDatabaseService
    {
        private SQLiteAsyncConnection _database;

        // Veritabanı dosyasının yolu
        public static string DbPath => Path.Combine(FileSystem.AppDataDirectory, "deprem_takip.db3");

        async Task Init()
        {
            if (_database != null)
                return;

            _database = new SQLiteAsyncConnection(DbPath);
            // Tabloyu oluştur (varsa bir şey yapmaz)
            await _database.CreateTableAsync<FollowedCitySetting>();
        }

        public async Task<List<FollowedCitySetting>> GetCitiesAsync()
        {
            await Init();
            return await _database.Table<FollowedCitySetting>().ToListAsync();
        }

        public async Task AddCityAsync(FollowedCitySetting city)
        {
            await Init();
            await _database.InsertAsync(city);
        }

        public async Task RemoveCityAsync(FollowedCitySetting city)
        {
            await Init();
            await _database.DeleteAsync(city);
        }
    }
}