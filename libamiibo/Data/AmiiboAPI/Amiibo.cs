using System.Text.Json.Serialization;

namespace LibAmiibo.Data.AmiiboAPI
{
    public class Amiibo
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("release")]
        public Dictionary<Region, string> Release { get; set; } = new Dictionary<Region, string>();
    }
}
