using DepremTakipAPP.Models;
using Newtonsoft.Json;
using System.Diagnostics;

namespace DepremTakipAPP.Services
{
    public class KandilliApiService
    {
        private readonly HttpClient _httpClient = new HttpClient();

        // Kandilli Adresi
        private const string ApiUrl = "https://api.orhanaydogdu.com.tr/deprem/kandilli/live?limit=25";

        public async Task<List<Earthquake>> GetLatestEarthquakesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(ApiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();

                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(jsonString);

                    return apiResponse?.Result ?? new List<Earthquake>();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"API Hatası (Kandilli): {ex.Message}");
            }
            return new List<Earthquake>();
        }
    }
}