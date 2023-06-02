/*
 * Copyright (C) 2018 Benjamin Krämer
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
using LibAmiibo.Attributes;
using LibAmiibo.Helper;

namespace LibAmiibo.Data.AppData.Games
{
    [AppID(0x00132600)]
    [AppDataInitializationTitleID("0004000000132800")]
    [AppDataInitializationTitleID("0004000000132600")]
    [AppDataInitializationTitleID("0004000000132700")]
    public class MarioLuigiPaperJam : IGame
    {
        public enum CharacterCheckValue
        {
            Toad = 'K',
            Luigi = 'L',
            Mario = 'M',
            Peach = 'P',
            Bowser = 'T',
            Yoshi = 'Y'
        }

        public enum CardStateValue
        {
            Disabled = 0b00,
            Enabled = 0b01,
            Sparkling = 0b11
        }

        private ArraySegment<byte> AppData { get; set; }

        public ArraySegment<byte> PublisherTag
        {
            get => new ArraySegment<byte>(this.AppData.Array, this.AppData.Offset + 0x00, 0x07);
            set => this.PublisherTag.CopyFrom(value);
        }

        public CharacterCheckValue CharacterCheck
        {
            get => (CharacterCheckValue)this.AppData.Array[this.AppData.Offset + 0x07];
            set => this.AppData.Array[this.AppData.Offset + 0x07] = (byte)value;
        }

        public ArraySegment<byte> OwnerTag
        {
            get => new ArraySegment<byte>(this.AppData.Array, this.AppData.Offset + 0x08, 0x08);
            set => this.OwnerTag.CopyFrom(value);
        }

        private CardStateValue GetCardState(int cardId)
        {
            var offset = this.AppData.Offset + 0x014 + (cardId / 4);
            var shift = (cardId % 4) * 2;
            var mask = (byte)(0b11 << shift);
            var value = this.AppData.Array[offset] & mask;
            return (CardStateValue)(value >> shift);
        }

        private void SetCardState(int cardId, CardStateValue value)
        {
            var offset = this.AppData.Offset + 0x014 + (cardId / 4);
            var shift = (cardId % 4) * 2;
            var mask = (byte)(0b11 << shift);
            var shiftValue = (byte)((byte)value << shift);
            this.AppData.Array[offset] &= (byte)~mask;
            this.AppData.Array[offset] |= shiftValue;
        }

        public MarioLuigiPaperJam(ArraySegment<byte> appData)
        {
            this.AppData = appData;
        }

        [SupportedGame(typeof(MarioLuigiPaperJam))]
        public class Initializer : IAppDataInitializer
        {
            private static readonly Dictionary<int, CharacterCheckValue> CHARACTER_CHECK_MAP = new Dictionary<int, CharacterCheckValue>
            {
                { 0x0000,   CharacterCheckValue.Mario },
                { 0x0001,   CharacterCheckValue.Luigi },
                { 0x0002,   CharacterCheckValue.Peach },
                { 0x0003,   CharacterCheckValue.Yoshi },
                { 0x0005,   CharacterCheckValue.Bowser },
                { 0x000A,   CharacterCheckValue.Toad },
            };

            public void InitializeAppData(AmiiboTag tag)
            {
                this.ThrowOnInvalidAppId(tag);
                var game = new MarioLuigiPaperJam(tag.AppDataBuffer);
                game.AppData.CopyFrom(new byte[0x20]); // TODO: Use for-loop and create extension method
                game.PublisherTag.CopyFrom(Encoding.ASCII.GetBytes("MILLION"));
                // TODO: how is OwnerTag calculated? Based on settings-CRC32?
                game.OwnerTag.CopyFrom(new byte[] { 0x3d, 0x0d, 0x2f, 0xc9, 0x34, 0x31, 0x67, 0xef });

                if (CHARACTER_CHECK_MAP.TryGetValue(tag.Amiibo.CharacterId, out CharacterCheckValue tmpCharCheck))
                {
                    game.CharacterCheck = tmpCharCheck;
                }
            }
        }

        #region Level 1

        [Cheat(CheatType.MultiDropDown, "Level 1", "Card 1")]
        public CardStateValue Level1Card1State
        {
            get => this.GetCardState(0);
            set => this.SetCardState(0, value);
        }

        [Cheat(CheatType.MultiDropDown, "Level 1", "Card 2")]
        public CardStateValue Level1Card2State
        {
            get => this.GetCardState(1);
            set => this.SetCardState(1, value);
        }

        [Cheat(CheatType.MultiDropDown, "Level 1", "Card 3")]
        public CardStateValue Level1Card3State
        {
            get => this.GetCardState(2);
            set => this.SetCardState(2, value);
        }

        [Cheat(CheatType.MultiDropDown, "Level 1", "Card 4")]
        public CardStateValue Level1Card4State
        {
            get => this.GetCardState(3);
            set => this.SetCardState(3, value);
        }

        [Cheat(CheatType.MultiDropDown, "Level 1", "Card 5")]
        public CardStateValue Level1Card5State
        {
            get => this.GetCardState(4);
            set => this.SetCardState(4, value);
        }

        [Cheat(CheatType.MultiDropDown, "Level 1", "Card 6")]
        public CardStateValue Level1Card6State
        {
            get => this.GetCardState(5);
            set => this.SetCardState(5, value);
        }

        [Cheat(CheatType.MultiDropDown, "Level 1", "Card 7")]
        public CardStateValue Level1Card7State
        {
            get => this.GetCardState(6);
            set => this.SetCardState(6, value);
        }

        [Cheat(CheatType.MultiDropDown, "Level 1", "Card 8")]
        public CardStateValue Level1Card8State
        {
            get => this.GetCardState(7);
            set => this.SetCardState(7, value);
        }

        [Cheat(CheatType.MultiDropDown, "Level 1", "Card 9")]
        public CardStateValue Level1Card9State
        {
            get => this.GetCardState(8);
            set => this.SetCardState(8, value);
        }

        #endregion

        #region Level 1 - Combo

        [Cheat(CheatType.MultiDropDown, "Level 1 - Combo", "Card 1")]
        public CardStateValue Level1Combo1State
        {
            get => this.GetCardState(9);
            set => this.SetCardState(9, value);
        }

        [Cheat(CheatType.MultiDropDown, "Level 1 - Combo", "Card 2")]
        public CardStateValue Level1Combo2State
        {
            get => this.GetCardState(10);
            set => this.SetCardState(10, value);
        }

        [Cheat(CheatType.MultiDropDown, "Level 1 - Combo", "Card 3")]
        public CardStateValue Level1Combo3State
        {
            get => this.GetCardState(11);
            set => this.SetCardState(11, value);
        }

        [Cheat(CheatType.MultiDropDown, "Level 1 - Combo", "Card 4")]
        public CardStateValue Level1Combo4State
        {
            get => this.GetCardState(12);
            set => this.SetCardState(12, value);
        }

        #endregion

        #region Level 2

        [Cheat(CheatType.MultiDropDown, "Level 2", "Card 1")]
        public CardStateValue Level2Card1State
        {
            get => this.GetCardState(13);
            set => this.SetCardState(13, value);
        }

        [Cheat(CheatType.MultiDropDown, "Level 2", "Card 2")]
        public CardStateValue Level2Card2State
        {
            get => this.GetCardState(14);
            set => this.SetCardState(14, value);
        }

        [Cheat(CheatType.MultiDropDown, "Level 2", "Card 3")]
        public CardStateValue Level2Card3State
        {
            get => this.GetCardState(15);
            set => this.SetCardState(15, value);
        }

        [Cheat(CheatType.MultiDropDown, "Level 2", "Card 4")]
        public CardStateValue Level2Card4State
        {
            get => this.GetCardState(16);
            set => this.SetCardState(16, value);
        }

        [Cheat(CheatType.MultiDropDown, "Level 2", "Card 5")]
        public CardStateValue Level2Card5State
        {
            get => this.GetCardState(17);
            set => this.SetCardState(17, value);
        }

        [Cheat(CheatType.MultiDropDown, "Level 2", "Card 6")]
        public CardStateValue Level2Card6State
        {
            get => this.GetCardState(18);
            set => this.SetCardState(18, value);
        }

        [Cheat(CheatType.MultiDropDown, "Level 2", "Card 7")]
        public CardStateValue Level2Card7State
        {
            get => this.GetCardState(19);
            set => this.SetCardState(19, value);
        }

        [Cheat(CheatType.MultiDropDown, "Level 2", "Card 8")]
        public CardStateValue Level2Card8State
        {
            get => this.GetCardState(20);
            set => this.SetCardState(20, value);
        }

        [Cheat(CheatType.MultiDropDown, "Level 2", "Card 9")]
        public CardStateValue Level2Card9State
        {
            get => this.GetCardState(21);
            set => this.SetCardState(21, value);
        }

        #endregion

        #region Level 2 - Combo

        [Cheat(CheatType.MultiDropDown, "Level 2 - Combo", "Card 1")]
        public CardStateValue Level2Combo1State
        {
            get => this.GetCardState(22);
            set => this.SetCardState(22, value);
        }

        [Cheat(CheatType.MultiDropDown, "Level 2 - Combo", "Card 2")]
        public CardStateValue Level2Combo2State
        {
            get => this.GetCardState(23);
            set => this.SetCardState(23, value);
        }

        [Cheat(CheatType.MultiDropDown, "Level 2 - Combo", "Card 3")]
        public CardStateValue Level2Combo3State
        {
            get => this.GetCardState(24);
            set => this.SetCardState(24, value);
        }

        [Cheat(CheatType.MultiDropDown, "Level 2 - Combo", "Card 4")]
        public CardStateValue Level2Combo4State
        {
            get => this.GetCardState(25);
            set => this.SetCardState(25, value);
        }

        #endregion

        #region Level 3

        [Cheat(CheatType.MultiDropDown, "Level 3", "Card")]
        public CardStateValue Level3CardState
        {
            get => this.GetCardState(26);
            set => this.SetCardState(26, value);
        }

        #endregion
    }
}
