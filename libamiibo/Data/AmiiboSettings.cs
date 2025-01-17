﻿/*
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

using LibAmiibo.Helper;

namespace LibAmiibo.Data
{
    public class AmiiboSettings
    {
        public ArraySegment<byte> Data { get; }

        private IList<byte> CryptoBufferList => this.Data;

        public AmiiboUserData UserData { get; }

        public AmiiboAppData AppData { get; }

        public Status Status
        {
            get => (Status)(this.CryptoBufferList[0] & 0x30);
            set
            {
                var tmp = (int)this.CryptoBufferList[0];
                tmp &= ~0x30;
                tmp |= (int)value & 0x30;
                this.CryptoBufferList[0] = (byte)tmp;
            }
        }

        public ushort CrcUpdateCounter
        {
            get => NtagHelper.UInt16FromTag(this.Data, 0x02);
            set => NtagHelper.UInt16ToTag(this.Data, 0x02, value);
        }

        public ushort LastModifiedDateValue
        {
            get => NtagHelper.UInt16FromTag(this.Data, 0x06);
            set => NtagHelper.UInt16ToTag(this.Data, 0x06, value);
        }

        public DateTime LastModifiedDate
        {
            get => NtagHelper.DateTimeFromTag(this.LastModifiedDateValue);
            set => this.LastModifiedDateValue = NtagHelper.DateTimeToTag(value);
        }

        // TODO: This is the unique console hash
        public uint CRC32
        {
            get => NtagHelper.UInt32FromTag(this.Data, 0x08);
            set => NtagHelper.UInt32ToTag(this.Data, 0x08, value);
        }

        public ushort WriteCounter
        {
            get => NtagHelper.UInt16FromTag(this.Data, 0x88);
            set => NtagHelper.UInt16ToTag(this.Data, 0x88, value);
        }

        public ArraySegment<byte> Unknown8EBytes
        {
            get => new ArraySegment<byte>(this.Data.Array, this.Data.Offset + 0x8E, 0x02);
            set => this.Unknown8EBytes.CopyFrom(value);
        }

        public ArraySegment<byte> Signature
        {
            get => new ArraySegment<byte>(this.Data.Array, this.Data.Offset + 0x90, 0x20);
            set => this.Signature.CopyFrom(value);
        }

        public AmiiboSettings(ArraySegment<byte> cryptoData, ArraySegment<byte> appData)
        {
            this.Data = cryptoData;
            this.UserData = new AmiiboUserData(cryptoData);
            this.AppData = new AmiiboAppData(cryptoData, appData);
        }
    }
}
