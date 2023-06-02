using System.Text.Json.Serialization;

namespace LibAmiibo.Data.AmiiboAPI
{
    public class AmiiboApiData
    {
        [JsonPropertyName("amiibo_series")]
        public Dictionary<string, string> AmiiboSeries { get; set; } = new Dictionary<string, string>();

        [JsonPropertyName("amiibos")]
        public Dictionary<string, Amiibo> Amiibos { get; set; } = new Dictionary<string, Amiibo>();

        [JsonPropertyName("characters")]
        public Dictionary<string, string> Characters { get; set; } = new Dictionary<string, string>();

        [JsonPropertyName("game_series")]
        public Dictionary<string, string> GameSeries { get; set; } = new Dictionary<string, string>();

        [JsonPropertyName("types")]
        public Dictionary<string, string> Types { get; set; } = new Dictionary<string, string>();
    }
}
