using LibAmiibo.Data;
using LibAmiibo.Data.AmiiboAPI;
using LibAmiibo.Interfaces;
using LibAmiibo.Services;

namespace LibAmiibo
{
    public class LibAmiibo
    {
        private readonly AmiiboApiService apiService;
        private readonly IAmiiboInfoDataProvider amiiboInfoDataProvider;
        private readonly AmiiboManager amiiboManager;

        public bool IsLocalAmiiboDataAvailable => this.apiService?.IsDataAvailable ?? false;

        public AmiiboApiService GetApiService => this.apiService;

        public LibAmiibo(string amiiboKeyFile = null)
        {
            this.apiService = new AmiiboApiService();
            this.amiiboInfoDataProvider = new AmiiboInfoDataProvider(this.apiService);
            this.amiiboManager = new AmiiboManager(this.amiiboInfoDataProvider);

            if (!string.IsNullOrEmpty(amiiboKeyFile))
            {
                if (!File.Exists(amiiboKeyFile))
                {
                    throw new ArgumentException($"No Amiibo key found under '{nameof(amiiboKeyFile)}'");
                }

                this.amiiboManager.LoadKeyFile(amiiboKeyFile);
            }
        }

        public async Task UpdateLocalAmiiboData()
        {
            await this.apiService.DownloadListsData();
            await this.amiiboInfoDataProvider.Refresh();
        }

        public async Task<bool> RefreshInfoDataProvider() => await this.amiiboInfoDataProvider.Refresh();

        public bool IsAmiiboKeyProvided => this.amiiboManager?.IsAmiiboKeyProvided ?? false;

        public async Task UpdateMissingLocalAmiiboImages() => await this.apiService.DownloadMissingImage((await this.GetAmiiboApiDataAsync()).Amiibos.Keys.ToArray());

        public async Task<string> GetLocalAmiiboImage(string hexId, bool downloadIfMissing = true) => await this.apiService.GetAmiiboImage(hexId, downloadIfMissing);

        public async Task<AmiiboApiData> GetAmiiboApiDataAsync() => await this.apiService.GetAmiiboApiDataAsync();

        public AmiiboApiData GetAmiiboApiData() => this.apiService.GetAmiiboApiData();

        public AmiiboTag DecryptTag(byte[] data) => this.amiiboManager.DecryptTag(data);

        public byte[] EncryptTag(AmiiboTag tag) => this.amiiboManager.EncryptTag(tag);

        public AmiiboTag ReadEncryptedTag(byte[] data) => this.amiiboManager.ReadEncryptedTag(data);
    }
}
