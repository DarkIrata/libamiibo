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

using LibAmiibo.Interfaces;

namespace LibAmiibo.Data
{
    public class Amiibo
    {
        private readonly IAmiiboInfoDataProvider infoDataProvider;

        public string StatueId { get; }

        public bool IsDataComplete => this.StatueNameInternal != null &&
                                      this.GameSeriesNameInternal != null &&
                                      this.CharacterNameInternal != null &&
                                      this.SubCharacterNameInternal != null &&
                                      this.TypeNameInternal != null &&
                                      this.AmiiboSetNameInternal != null;

        private string StatueNameInternal => this.infoDataProvider.GetAmiiboName(this.StatueId);

        private string GameSeriesNameInternal => this.infoDataProvider.GetGameSeriesName(this.GameSeriesId);

        private string CharacterNameInternal => this.infoDataProvider.GetCharacterName(this.CharacterId);

        private string SubCharacterNameInternal => this.infoDataProvider.GetSubCharacterName(this.SubCharacterId);

        private string TypeNameInternal => this.infoDataProvider.GetTypeName(this.TypeId);

        private string AmiiboSetNameInternal => this.infoDataProvider.GetAmiiboSetName(this.AmiiboSetId);

        public string StatueName => this.StatueNameInternal ?? "Unknown " + this.AmiiboNo;

        public int AmiiboNo => int.Parse(this.StatueId.Substring(8, 4), System.Globalization.NumberStyles.HexNumber);

        // Game series id only uses 10 bits:
        public int GameSeriesId => int.Parse(this.StatueId.Substring(0, 3), System.Globalization.NumberStyles.HexNumber);

        public string GameSeriesName => this.GameSeriesNameInternal ?? "Unknown " + this.GameSeriesId;

        // Character number in series is defined by the last 6 bits:
        public byte CharacterNumberInGameSeries => (byte)(byte.Parse(this.StatueId.Substring(3, 1), System.Globalization.NumberStyles.HexNumber) & 0x3F);

        public int CharacterId => int.Parse(this.StatueId.Substring(0, 4), System.Globalization.NumberStyles.HexNumber);

        public string CharacterName => this.CharacterNameInternal ?? "Unknown " + this.CharacterId;

        public byte CharacterVariant => byte.Parse(this.StatueId.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

        public long SubCharacterId => long.Parse(this.StatueId.Substring(0, 6), System.Globalization.NumberStyles.HexNumber);

        public string SubCharacterName
        {
            get
            {
                if (this.CharacterVariant == 0x00)
                {
                    return "Regular";
                }

                return this.SubCharacterNameInternal ?? "Unknown " + this.SubCharacterId;
            }
        }

        public byte TypeId => byte.Parse(this.StatueId.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);

        public string ToyTypeName => this.TypeNameInternal ?? "Unknown " + this.TypeId;

        public byte AmiiboSetId => byte.Parse(this.StatueId.Substring(12, 2), System.Globalization.NumberStyles.HexNumber);

        public string AmiiboSetName => this.AmiiboSetNameInternal ?? "Unknown " + this.AmiiboSetId;

        public string RetailName
        {
            get
            {
                // If known, use retail name:
                string retailName = this.StatueNameInternal;
                if (retailName != null && (this.AmiiboNo != 0 || this.CharacterId == 0))
                {
                    return retailName;
                }

                // Always use the characters name if known, or it's number in the series:
                retailName = this.CharacterNameInternal ?? ("Char#" + this.CharacterNumberInGameSeries);

                // Add the game series name (or id if unknown):
                retailName += " (" + (this.GameSeriesNameInternal ?? "series " + this.GameSeriesId);

                // Only add the variant if not the standard one:
                if (this.CharacterVariant > 0)
                {
                    // Try to get the subcharacter name or use it's number instead
                    retailName += ", " + (this.SubCharacterNameInternal ?? "variant " + this.CharacterVariant);
                }
                retailName += ")";

                return retailName;
            }
        }

        private Amiibo(IList<byte> internalTag, IAmiiboInfoDataProvider infoDataProvider)
        {
            this.StatueId = string.Format("{0:X2}{1:X2}{2:X2}{3:X2}{4:X2}{5:X2}{6:X2}{7:X2}",
                    internalTag[0x1DC], internalTag[0x1DD], internalTag[0x1DE], internalTag[0x1DF], internalTag[0x1E0], internalTag[0x1E1], internalTag[0x1E2], internalTag[0x1E3]);
            this.infoDataProvider = infoDataProvider;
        }

        private Amiibo(string statueId, IAmiiboInfoDataProvider infoDataProvider)
        {
            this.StatueId = statueId;
            this.infoDataProvider = infoDataProvider;
        }

        public static Amiibo FromInternalTag(ArraySegment<byte> data, IAmiiboInfoDataProvider infoDataProvider)
            => new Amiibo(data, infoDataProvider);

        public static Amiibo FromNtagData(byte[] data, IAmiiboInfoDataProvider infoDataProvider) => new Amiibo(string.Format("{0:X2}{1:X2}{2:X2}{3:X2}{4:X2}{5:X2}{6:X2}{7:X2}",
                data[84], data[85], data[86], data[87], data[88], data[89], data[90], data[91]), infoDataProvider);

        public static Amiibo FromStatueId(byte[] statueId, IAmiiboInfoDataProvider infoDataProvider) => new Amiibo(string.Format("{0:X2}{1:X2}{2:X2}{3:X2}{4:X2}{5:X2}{6:X2}{7:X2}",
                    statueId[0], statueId[1], statueId[2], statueId[3], statueId[4], statueId[5], statueId[6], statueId[7]), infoDataProvider);

        public override string ToString() => this.RetailName;
    }
}
