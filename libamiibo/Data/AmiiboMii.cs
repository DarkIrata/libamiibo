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

using System.Text;
using LibAmiibo.Helper;

namespace LibAmiibo.Data
{
    public class AmiiboMii
    {
        public ArraySegment<byte> Data { get; }

        private IList<byte> MiiBufferList => this.Data;

        public uint MiiID
        {
            get => NtagHelper.UInt32FromTag(this.Data, 0x00);
            set => NtagHelper.UInt32ToTag(this.Data, 0x00, value);
        }

        public ulong SystemID
        {
            get => NtagHelper.UInt64FromTag(this.Data, 0x04);
            set => NtagHelper.UInt64ToTag(this.Data, 0x04, value);
        }

        public uint SpecialnessAndDateOfCreation
        {
            get => NtagHelper.UInt32FromTag(this.Data, 0x0C);
            set => NtagHelper.UInt32ToTag(this.Data, 0x0C, value);
        }

        private ArraySegment<byte> CreatorsMAC
        {
            get => new ArraySegment<byte>(this.Data.Array, this.Data.Offset + 0x10, 0x06);
            set => this.CreatorsMAC.CopyFrom(value);
        }

        public ushort BirthdaySexShirtFavorite
        {
            get => NtagHelper.UInt16FromTag(this.Data, 0x18);
            set => NtagHelper.UInt16ToTag(this.Data, 0x18, value);
        }

        public string Nickname
        {
            get => MarshalUtil.CleanInput(Encoding.Unicode.GetString(this.Data.Array, this.Data.Offset + 0x1A, 0x14));
            set
            {
                if (value.Length > 10)
                {
                    value = value[..10];
                }

                this.NicknameBuffer.CopyFrom(Encoding.Unicode.GetBytes(MarshalUtil.CleanOutput(value)));
            }
        }

        private ArraySegment<byte> NicknameBuffer
        {
            get => new ArraySegment<byte>(this.Data.Array, this.Data.Offset + 0x1A, 0x14);
            set => this.NicknameBuffer.CopyFrom(value);
        }

        public ushort WidthAndHeight
        {
            get => NtagHelper.UInt16FromTag(this.Data, 0x2E);
            set => NtagHelper.UInt16ToTag(this.Data, 0x2E, value);
        }

        public byte SharingFaceshapeSkincolor
        {
            get => this.MiiBufferList[0x30];
            set => this.MiiBufferList[0x30] = value;
        }

        public byte WrinklesMakeup
        {
            get => this.MiiBufferList[0x31];
            set => this.MiiBufferList[0x31] = value;
        }

        public byte Hairstyle
        {
            get => this.MiiBufferList[0x32];
            set => this.MiiBufferList[0x32] = value;
        }

        public byte HaircolorFliphair
        {
            get => this.MiiBufferList[0x33];
            set => this.MiiBufferList[0x33] = value;
        }

        public ArraySegment<byte> Unknown34Bytes
        {
            get => new ArraySegment<byte>(this.Data.Array, this.Data.Offset + 0x34, 0x04);
            set => this.Unknown34Bytes.CopyFrom(value);
        }

        public byte EyebrowStyleAndColor
        {
            get => this.MiiBufferList[0x38];
            set => this.MiiBufferList[0x38] = value;
        }

        public byte EyebrowScale
        {
            get => this.MiiBufferList[0x39];
            set => this.MiiBufferList[0x39] = value;
        }

        public byte EyebrowRotationAndXSpacing
        {
            get => this.MiiBufferList[0x3A];
            set => this.MiiBufferList[0x3A] = value;
        }

        public byte EyebrowYPosition
        {
            get => this.MiiBufferList[0x3B];
            set => this.MiiBufferList[0x3B] = value;
        }

        public ArraySegment<byte> Unknown3CBytes
        {
            get => new ArraySegment<byte>(this.Data.Array, this.Data.Offset + 0x3C, 0x04);
            set => this.Unknown3CBytes.CopyFrom(value);
        }

        public byte AllowCopying
        {
            get => this.MiiBufferList[0x40];
            set => this.MiiBufferList[0x40] = value;
        }

        public ArraySegment<byte> Unknown41Bytes
        {
            get => new ArraySegment<byte>(this.Data.Array, this.Data.Offset + 0x41, 0x07);
            set => this.Unknown41Bytes.CopyFrom(value);
        }

        public string AuthorNickname
        {
            get => MarshalUtil.CleanInput(Encoding.Unicode.GetString(this.Data.Array, this.Data.Offset + 0x48, 0x14));
            set => this.AuthorNicknameBuffer.CopyFrom(Encoding.Unicode.GetBytes(MarshalUtil.CleanOutput(value)));
        }

        private ArraySegment<byte> AuthorNicknameBuffer
        {
            get => new ArraySegment<byte>(this.Data.Array, this.Data.Offset + 0x48, 0x14);
            set => this.AuthorNicknameBuffer.CopyFrom(value);
        }

        public AmiiboMii(ArraySegment<byte> miiData)
        {
            this.Data = miiData;
        }
    }
}
