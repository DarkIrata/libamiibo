using System.Text.Json;
using LibAmiibo.Data.AmiiboAPI;

namespace LibAmiibo.Services
{
    public class AmiiboApiService
    {
        private const string AmiiboApiListsDataUrl = "https://raw.githubusercontent.com/N3evin/AmiiboAPI/master/database/amiibo.json";
        private const string AmiiboApiImageBaseUrl = "https://raw.githubusercontent.com/N3evin/AmiiboAPI/master/images/";
        private const string DataDir = "./AmiiboApi/";
        private readonly string ImagesDir = DataDir + "Images/";
        private readonly string AmiibosApiListsDataFile = DataDir + "amiibo.json";

        public bool IsDataAvailable => File.Exists(this.AmiibosApiListsDataFile);

        public AmiiboApiService()
        {
            Directory.CreateDirectory(DataDir);
            Directory.CreateDirectory(this.ImagesDir);
        }

        public async Task<(bool Successful, string Error)> DownloadListsData()
        {
            try
            {
                using var wc = new HttpClient();
                using (var fw = File.OpenWrite(this.AmiibosApiListsDataFile))
                {
                    var response = await wc.GetAsync(AmiiboApiListsDataUrl);
                    await response.Content.CopyToAsync(fw);
                }

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        public async Task DownloadImage(string hexId) => await this.DownloadImage(new[] { hexId });

        public async Task DownloadImage(string[] hexIds)
        {
            var parallelOptions = new ParallelOptions()
            {
                MaxDegreeOfParallelism = 5
            };

            await Parallel.ForEachAsync(hexIds, parallelOptions, async (hexId, token) =>
            {
                var filePath = this.GetAmiiboImagePath(hexId);
                var filePathTmp = filePath + ".tmp";
                var remove = false;
                try
                {
                    using var wc = new HttpClient();
                    using (var fw = File.OpenWrite(filePathTmp))
                    {
                        var response = await wc.GetAsync(AmiiboApiImageBaseUrl + Path.GetFileName(filePath), token);
                        await response.Content.CopyToAsync(fw);
                    }

                    if (File.Exists(filePathTmp))
                    {
                        var info = new FileInfo(filePathTmp);
                        if (info.Length == 0)
                        {
                            remove = true;
                        }
                    }
                }
                catch
                {
                    remove = true;
                }

                if (File.Exists(filePathTmp) && remove)
                {
                    File.Delete(filePathTmp);
                }

                if (File.Exists(filePathTmp))
                {
                    File.Move(filePathTmp, filePath);
                }
            });
        }

        public string GetAmiiboImagePath(string hexId)
        {
            var filename = this.AmiiboIdToImageFilename(hexId);
            return this.ImagesDir + filename;
        }

        public string AmiiboIdToImageFilename(string hexId)
        {
            if (hexId.StartsWith("0x"))
            {
                hexId = hexId.Substring(2);
            }

            if (hexId.Length != 16)
            {
                throw new ArgumentException($"{hexId} is not 16 chars (18 chars with beginning 0x) long.");
            }

            hexId = hexId.ToLower();
            return $"icon_{hexId.Substring(0, 8)}-{hexId.Substring(8)}.png";
        }

        public async Task<AmiiboApiData> GetAmiiboApiDataAsync()
        {
            if (!File.Exists(this.AmiibosApiListsDataFile))
            {
                return null;
            }

            using var fr = File.OpenRead(this.AmiibosApiListsDataFile);
            return await JsonSerializer.DeserializeAsync<AmiiboApiData>(fr);
        }

        public AmiiboApiData GetAmiiboApiData()
        {
            if (!File.Exists(this.AmiibosApiListsDataFile))
            {
                return null;
            }

            using var fr = File.OpenRead(this.AmiibosApiListsDataFile);
            return JsonSerializer.Deserialize<AmiiboApiData>(fr);
        }

        internal async Task<string> GetAmiiboImage(string hexId, bool downloadIfMissing)
        {
            var filename = this.AmiiboIdToImageFilename(hexId);
            if (!File.Exists(this.ImagesDir + filename))
            {
                if (downloadIfMissing)
                {
                    await this.DownloadImage(hexId);
                }
                else
                {
                    return null;
                }
            }

            return this.ImagesDir + filename;
        }

        internal async Task DownloadMissingImage(string[] hexIds)
        {
            var missing = new List<string>();
            foreach (var id in hexIds)
            {
                var filename = this.AmiiboIdToImageFilename(id);
                if (!File.Exists(this.ImagesDir + filename))
                {
                    missing.Add(id);
                }
            }

            await this.DownloadImage(missing.ToArray());
        }
    }
}
