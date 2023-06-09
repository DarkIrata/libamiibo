using LibAmiibo.Data.AmiiboAPI;
using LibAmiibo.Interfaces;

namespace LibAmiibo.Services
{
    public class AmiiboInfoDataProvider : IAmiiboInfoDataProvider
    {
        private AmiiboApiService apiService;
        private AmiiboApiData apiData;

        public AmiiboInfoDataProvider(AmiiboApiService apiService)
        {
            this.apiService = apiService;
        }

        public async Task<bool> Refresh()
        {
            if (this.apiService.IsDataAvailable)
            {
                this.apiData = await this.apiService.GetAmiiboApiData();
            }

            return this.apiData != null;
        }

        public string GetAmiiboName(long id) => this.GetAmiiboName(this.IdToHexId(id, 16));

        public string GetGameSeriesName(int id) => this.GetGameSeriesName(this.IdToHexId(id, 3));

        public string GetCharacterName(int id) => this.GetCharacterName(this.IdToHexId(id, 4));

        public string GetSubCharacterName(long id) => this.GetSubCharacterName(this.IdToHexId(id, 6));

        public string GetTypeName(int id) => this.GetTypeName(this.IdToHexId(id, 2));

        public string GetAmiiboSetName(int id) => this.GetAmiiboSetName(this.IdToHexId(id, 2));

        public string GetAmiiboName(string hexId) => this.GetEntry<Amiibo>(this.apiData?.Amiibos, hexId)?.Name;

        public string GetGameSeriesName(string hexId) => this.GetEntry<string>(this.apiData?.GameSeries, hexId);

        public string GetCharacterName(string hexId) => this.GetEntry<string>(this.apiData?.Characters, hexId);

        public string GetSubCharacterName(string hexId)
        {
            var amiiboKV = this.apiData?.Amiibos.FirstOrDefault(a => a.Key.StartsWith(hexId, StringComparison.OrdinalIgnoreCase));
            var name = amiiboKV?.Value?.Name;
            if (string.IsNullOrEmpty(name))
            {
                return name;
            }

            var splitterIndex = name.LastIndexOf('-') + 1;
            if (splitterIndex > 0 && splitterIndex < name.Length)
            {
                name = name.Substring(splitterIndex);
            }

            return name.Trim();
        }

        public string GetTypeName(string hexId) => this.GetEntry<string>(this.apiData?.Types, hexId);

        public string GetAmiiboSetName(string hexId) => this.GetEntry<string>(this.apiData?.AmiiboSeries, hexId);

        private string IdToHexId(long id, short length) => "0x" + id.ToString($"X{length}");

        private T GetEntry<T>(IReadOnlyDictionary<string, T> dict, string hexId)
            where T : class
        {
            if (!hexId.StartsWith("0x"))
            {
                hexId = "0x" + hexId;
            }

            if (dict != null && dict.TryGetValue(hexId, out T value))
            {
                return value;
            }

            return null;
        }
    }
}
