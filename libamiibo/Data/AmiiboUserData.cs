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

using System.Text;
using LibAmiibo.Helper;

namespace LibAmiibo.Data
{
    public class AmiiboUserData
    {
        public ArraySegment<byte> Data { get; }

        private IList<byte> CryptoBufferList => this.Data;

        public byte[] GetAmiiboSettingsBytes
        {
            get => new[]
                {
                    (byte) (this.CryptoBufferList[0] & 0x0F),
                    this.CryptoBufferList[1]
                };
            set
            {
                var tmp = (int)this.CryptoBufferList[0];
                tmp &= ~0x0F;
                tmp |= value[0] & 0x0F;
                this.CryptoBufferList[0] = (byte)tmp;
                this.CryptoBufferList[1] = value[1];
            }
        }

        // TODO: Add Country Code from 0x01

        public ushort SetupDateValue
        {
            get => NtagHelper.UInt16FromTag(this.Data, 0x04);
            set => NtagHelper.UInt16ToTag(this.Data, 0x04, value);
        }

        public DateTime SetupDate
        {
            get => NtagHelper.DateTimeFromTag(this.SetupDateValue);
            set => this.SetupDateValue = NtagHelper.DateTimeToTag(value);
        }

        public string Nickname
        {
            get => MarshalUtil.CleanInput(Encoding.BigEndianUnicode.GetString(this.Data.Array, this.Data.Offset + 0x0C, 0x14));
            set => this.NicknameBuffer.CopyFrom(Encoding.BigEndianUnicode.GetBytes(MarshalUtil.CleanOutput(value)));
        }

        private ArraySegment<byte> NicknameBuffer
        {
            get => new ArraySegment<byte>(this.Data.Array, this.Data.Offset + 0x0C, 0x14);
            set => this.NicknameBuffer.CopyFrom(value);
        }

        public AmiiboMii OwnerMii { get; }

        private ArraySegment<byte> OwnerMiiBuffer
        {
            get => new ArraySegment<byte>(this.Data.Array, this.Data.Offset + 0x20, 0x60);
            set => this.OwnerMiiBuffer.CopyFrom(value);
        }

        public AmiiboUserData(ArraySegment<byte> cryptoData)
        {
            this.Data = cryptoData;
            this.OwnerMii = new AmiiboMii(this.OwnerMiiBuffer);
        }
    }
}
