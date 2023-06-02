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

using LibAmiibo.Helper;
using LibAmiibo.IDBE;

namespace LibAmiibo.Data.AppData
{
    // TODO: Use http://3dsdb.com/, http://wiiubrew.org/wiki/Title_database and http://switchbrew.org/index.php?title=Title_list to resolve the titles
    public class Title
    {
        private IDBEContext context;
        public ArraySegment<byte> Data { get; }
        private IList<byte> DataList => this.Data;

        public IDBEContext Context
        {
            get
            {
                if (this.context == null)
                {
                    this.context = CDNUtils.DownloadTitleData(this);
                }
                return this.context;
            }
        }

        public Platform Platform => (Platform)NtagHelper.UInt16FromTag(this.Data, 0x00);

        // TODO: This is only correct for N3DS and WiiU
        /// <summary>
        /// Currently only correct for N3DS and WiiU
        /// </summary>
        public Category Category => (Category)NtagHelper.UInt16FromTag(this.Data, 0x02);

        public ulong TitleID => NtagHelper.UInt64FromTag(this.Data, 0x00);

        // TODO: This is only correct for N3DS and WiiU
        /// <summary>
        /// Currently only correct for N3DS and WiiU
        /// </summary>
        public uint UniqueID
        {
            get
            {
                var data = new byte[0x04];
                Array.Copy(this.Data.Array, this.Data.Offset + 0x04, data, 0x01, 0x03); // Offset by 0x02
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(data);
                }

                return BitConverter.ToUInt32(data, 0);
            }
        }

        // TODO: This is only correct for N3DS and WiiU
        /// <summary>
        /// Currently only correct for N3DS and WiiU
        /// </summary>
        public TitleType UniqueIDType => TitleTypeUtil.GetType(this.UniqueID);

        // TODO: This is only correct for N3DS and WiiU
        /// <summary>
        /// Currently only correct for N3DS and WiiU
        /// </summary>
        public Variation Variation => (Variation)this.DataList[0x07];

        private Title(ArraySegment<byte> data) => this.Data = data;

        public static Title FromTitleID(long data)
        {
            var formatted = BitConverter.GetBytes(data);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(formatted);
            }

            return FromTitleID(formatted);
        }

        public static Title FromTitleID(byte[] data) => new Title(new ArraySegment<byte>(data));

        public static Title FromTitleID(string data) => FromTitleID(NtagHelper.StringToByteArrayFastest(data));

        public static Title FromTitleID(ArraySegment<byte> data) => new Title(data);
    }
}
