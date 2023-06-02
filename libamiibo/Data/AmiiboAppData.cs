/*
 * Copyright (C) 2016 Benjamin Krämer
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

using LibAmiibo.Data.AppData;
using LibAmiibo.Data.AppData.Games;
using LibAmiibo.Helper;

namespace LibAmiibo.Data
{
    public class AmiiboAppData
    {
        public ArraySegment<byte> Data { get; }

        public ArraySegment<byte> AppData { get; }

        private IList<byte> CryptoBufferList => this.Data;

        private ArraySegment<byte> AppDataInitializationTitleIDBuffer
        {
            get => new ArraySegment<byte>(this.Data.Array, this.Data.Offset + 0x80, 0x08);
            set => this.AppDataInitializationTitleIDBuffer.CopyFrom(value);
        }

        public Title InitializationTitleID
        {
            get => Title.FromTitleID(this.AppDataInitializationTitleIDBuffer);
            set => this.AppDataInitializationTitleIDBuffer.CopyFrom(value.Data);
        }

        public uint AppID
        {
            get => NtagHelper.UInt32FromTag(this.Data, 0x8A);
            set => NtagHelper.UInt32ToTag(this.Data, 0x8A, value);
        }

        /// <summary>
        /// To set the game, use AmiiboTag.InitializeAppData()!
        /// </summary>
        public IGame Game => AppDataInitializerExtensions.GetGameForAmiiboAppData(this);

        public AmiiboAppData(ArraySegment<byte> cryptoData, ArraySegment<byte> appData)
        {
            this.Data = cryptoData;
            this.AppData = appData;
        }
    }
}
