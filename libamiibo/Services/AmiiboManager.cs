/*
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

using System.Security.Cryptography;
using LibAmiibo.Data;
using LibAmiibo.Encryption;
using LibAmiibo.Helper;
using LibAmiibo.Interfaces;

namespace LibAmiibo.Services
{
    public class AmiiboManager
    {
        private AmiiboKeys keys;
        private readonly IAmiiboInfoDataProvider infoDataProvider;

        public AmiiboManager(IAmiiboInfoDataProvider infoDataProvider)
        {
            this.infoDataProvider = infoDataProvider;
        }

        public void LoadKeyFile(string keyFilePath) => this.keys = AmiiboKeys.LoadKeys(keyFilePath);

        public AmiiboTag DecryptTag(byte[] data)
        {
            var decryptedData = new byte[NtagHelper.NFC3D_AMIIBO_SIZE];
            if (this.keys == null)
            {
                throw new InvalidOperationException("Cant decrypt without Amiibo key file");
            }

            if (!this.keys.Unpack(data, decryptedData))
            {
                throw new CryptographicException("Unpacking data was not successful");
            }

            return this.LoadDecryptedTag(decryptedData);
        }

        public byte[] EncryptTag(AmiiboTag tag)
        {
            byte[] encryptedData = new byte[NtagHelper.NFC3D_NTAG_SIZE];
            if (this.keys == null)
            {
                throw new InvalidOperationException("Cant encrypt without Amiibo key file");
            }

            this.keys.Pack(tag.Data.Array, encryptedData);
            return encryptedData;
        }

        public AmiiboTag ReadEncryptedTag(byte[] data) => this.LoadEncryptedTag(data);

        private AmiiboTag LoadDecryptedTag(byte[] data) => new AmiiboTag(data, this.infoDataProvider) { IsDecrypted = true };

        private AmiiboTag LoadEncryptedTag(byte[] data) => new AmiiboTag(new ArraySegment<byte>(NtagHelper.GetInternalTag(data)), this.infoDataProvider) { IsDecrypted = false };
    }
}
