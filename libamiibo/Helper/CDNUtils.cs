﻿/*
 * Copyright (C) 2016 Benjamin Krämer
 * Copyright (C) 2023 Wladislaw Batt
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System.Net;
using LibAmiibo.Data.AppData;
using LibAmiibo.IDBE;
using LibAmiibo.Encryption;

namespace LibAmiibo.Helper
{
    public static class CDNUtils
    {
        private static string TitleCacheDir = "./TitleCacheDir/";
        private static CDNKeys keys;

        static CDNUtils()
        {
            Directory.CreateDirectory(TitleCacheDir);

            // Accept nintendos certificate:
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors)
                => Equals("B48ECF5C04B9CBB18D6215D0AD51DAA113929DE8", certificate.GetCertHashString());
        }

        public static void LoadCDNKey(string path) => keys = CDNKeys.LoadKeys(path);

        public static byte[] DownloadTitleDataStream(Title title)
        {
            if (keys == null)
            {
                Console.Error.WriteLine("CDNKeys missing");
                return null;
            }

            string titleId = title.TitleID.ToString("X16").ToUpper();

            string metadataUrl;
            if (title.Platform == Platform.N3DS)
            {
                metadataUrl = $"https://idbe-ctr.cdn.nintendo.net/icondata/{10}/{titleId}.idbe";
            }
            else if (title.Platform == Platform.WiiU)
            {
                metadataUrl = $"https://idbe-wup.cdn.nintendo.net/icondata/{10}/{titleId}.idbe";
            }
            //else if (title.Platform == Platform.Switch) // TODO: Not working yet!
            //    metadataUrl = $"https://idbe-hac.cdn.nintendo.net/icondata/{10}/{titleId}.idbe";
            else
            {
                Console.Error.WriteLine("Wrong platform");
                return null;
            }

            byte[] data;
            try
            {
                using (var ms = new MemoryStream())
                using (var webClient = new HttpClient())
                {
                    var response = webClient.GetAsync(metadataUrl).Result;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        response.Content.CopyTo(ms, null, default);
                        data = ms.ToArray();
                    }
                    else
                    {
                        throw new WebException($"Error downloading data. Got result StatusCode '{response.StatusCode}'");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Problem downloading the data: " + ex.Message);
                return null;
            }

            var dataSkip2 = data.Skip(2).ToArray();
            var keyslot = data[1];

            return keys.DecryptIcon(dataSkip2, keyslot);
        }

        private static byte[] GetCachedStreamData(Title title)
        {
            var titleId = title.TitleID.ToString("X16").ToUpper();
            var cacheFileName = Path.Combine(TitleCacheDir, titleId);
            if (File.Exists(cacheFileName))
            {
                try
                {
                    return File.ReadAllBytes(cacheFileName);
                }
                catch { }
            }

            byte[] streamData = DownloadTitleDataStream(title);
            if (streamData != null)
            {
                File.WriteAllBytes(cacheFileName, streamData);
            }

            return streamData;
        }

        public static IDBEContext DownloadTitleData(Title title)
        {
            byte[] iconData = GetCachedStreamData(title);
            if (iconData == null)
            {
                return null;
            }

            if (title.Platform == Platform.WiiU)
            {
                using (var memoryStream = new MemoryStream(iconData))
                {
                    var context = new IDBEWiiUContext();
                    context.Open(memoryStream);
                    return context;
                }
            }
            if (title.Platform == Platform.N3DS)
            {
                // retrieve image (might get used later?)
                using (var memoryStream = new MemoryStream(iconData))
                {
                    var context = new IDBE3DSContext();
                    context.Open(memoryStream);
                    return context;
                }
            }

            return null;
        }
    }
}