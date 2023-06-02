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
using StbSharp;

namespace LibAmiibo.IDBE
{
    internal class IDBE3DSContext : IDBEContext
    {
        public Image SmallIcon;

        public Image LargeIcon;

        public override string FirstTitle(Localization localization)
            => MarshalUtil.CleanInput(Encoding.Unicode.GetString(this.Header.Descriptions[(int)localization].FirstTitle));

        public override string SecondTitle(Localization localization)
            => MarshalUtil.CleanInput(Encoding.Unicode.GetString(this.Header.Descriptions[(int)localization].SecondTitle));

        public override string Publisher(Localization localization)
            => MarshalUtil.CleanInput(Encoding.Unicode.GetString(this.Header.Descriptions[(int)localization].Publisher));

        public bool Open(Stream fs)
        {
            this.Header = MarshalUtil.ReadStruct<IDBEHeader>(fs);
            this.SmallIcon = ImageUtil.ReadImageFromStream(fs, 24, 24, ImageUtil.PixelFormat.RGB565);
            this.LargeIcon = ImageUtil.ReadImageFromStream(fs, 48, 48, ImageUtil.PixelFormat.RGB565);

            return true;
        }
    }
}