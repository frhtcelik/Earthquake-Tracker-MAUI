using Newtonsoft.Json;
using System.Collections.Generic;

namespace DepremTakipAPP.Models
{
    public class ApiResponse
    {
        [JsonProperty("status")]
        public bool Status { get; set; }

        [JsonProperty("result")]
        public List<Earthquake>? Result { get; set; }
    }
}