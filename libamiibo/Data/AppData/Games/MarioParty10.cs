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

using System.Text.Json.Serialization;
using LibAmiibo.Attributes;
using LibAmiibo.Helper;

namespace LibAmiibo.Data.AppData.Games
{
    [AppID(0x10161F00)]
    [AppDataInitializationTitleID("0005000E10162E00")]
    [AppDataInitializationTitleID("0005000E10161F00")]
    [AppDataInitializationTitleID("0005000E10162D00")]
    public class MarioParty10 : IGame
    {
        private const int TICKS_PER_SECOND = 62156250;
        private static readonly DateTime DateTimeBase = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private ArraySegment<byte> AppData { get; set; }

        [Flags]
        public enum DataValue
        {
            [JsonIgnore]
            Undefined = 0b000,
            Available = 0b001,
            New = 0b010,
            Selected = 0b100
        }

        public ArraySegment<byte> PublisherTag1
        {
            get => new ArraySegment<byte>(this.AppData.Array, this.AppData.Offset + 0x00, 0x02);
            set => this.PublisherTag1.CopyFrom(value);
        }

        public ArraySegment<byte> PublisherTag2
        {
            get => new ArraySegment<byte>(this.AppData.Array, this.AppData.Offset + 0x64, 0x04);
            set => this.PublisherTag2.CopyFrom(value);
        }

        public byte TimesPlayed
        {
            get => (byte)(this.AppData.Array[this.AppData.Offset + 0x49] - 3);
            set => this.AppData.Array[this.AppData.Offset + 0x49] = Math.Max((byte)(value + 3), byte.MaxValue);
        }

        private ulong LastDailyBonusDateValue
        {
            get => NtagHelper.UInt64FromTag(this.AppData, 0x50);
            set => NtagHelper.UInt64ToTag(this.AppData, 0x50, value);
        }

        public DateTime LastDailyBonusDate
        {
            get => DateTimeBase.AddSeconds(this.LastDailyBonusDateValue / TICKS_PER_SECOND);
            set => this.LastDailyBonusDateValue = (ulong)(value.Subtract(DateTimeBase).TotalSeconds * TICKS_PER_SECOND);
        }

        public uint SaveTag
        {
            get => NtagHelper.UInt32FromTag(this.AppData, 0x54);
            set => NtagHelper.UInt32ToTag(this.AppData, 0x54, value);
        }

        private DataValue GetDataState(int offset) => (DataValue)this.AppData.Array[this.AppData.Offset + offset];
        private void SetDataState(int offset, DataValue value) => this.AppData.Array[this.AppData.Offset + offset] = (byte)value;

        public MarioParty10(ArraySegment<byte> appData)
        {
            this.AppData = appData;
        }

        [SupportedGame(typeof(MarioParty10))]
        public class Initializer : IAppDataInitializer
        {
            public void InitializeAppData(AmiiboTag tag)
            {
                this.ThrowOnInvalidAppId(tag);
                var game = new MarioParty10(tag.AppDataBuffer);
                game.AppData.CopyFrom(new byte[0xD8]); // TODO: Use for-loop and create extension method
                game.PublisherTag1.CopyFrom(new byte[] { 0x05, 0x68 });
                game.PublisherTag2.CopyFrom(new byte[] { 0x27, 0x7D, 0xFB, 0x70 });
                game.TimesPlayed = 3;
            }
        }

        #region Tokens

        [Cheat(CheatType.MultiDropDown, "Tokens", "Mario Jump")]
        public DataValue TokenMarioJump
        {
            get => this.GetDataState(0x002);
            set => this.SetDataState(0x002, value);
        }

        [Cheat(CheatType.MultiDropDown, "Tokens", "Luigi Jump")]
        public DataValue TokenLuigiJump
        {
            get => this.GetDataState(0x003);
            set => this.SetDataState(0x003, value);
        }

        [Cheat(CheatType.MultiDropDown, "Tokens", "P Switch")]
        public DataValue TokenPSwitch
        {
            get => this.GetDataState(0x004);
            set => this.SetDataState(0x004, value);
        }

        [Cheat(CheatType.MultiDropDown, "Tokens", "Bowser Jr.")]
        public DataValue TokenBowserJr
        {
            get => this.GetDataState(0x005);
            set => this.SetDataState(0x005, value);
        }

        [Cheat(CheatType.MultiDropDown, "Tokens", "Plus 1")]
        public DataValue TokenPlus1
        {
            get => this.GetDataState(0x006);
            set => this.SetDataState(0x006, value);
        }

        [Cheat(CheatType.MultiDropDown, "Tokens", "1-2-3 Dice Block")]
        public DataValue Token1To3DiceBlock
        {
            get => this.GetDataState(0x007);
            set => this.SetDataState(0x007, value);
        }

        [Cheat(CheatType.MultiDropDown, "Tokens", "4-5-6 Dice Block")]
        public DataValue Token4To6DiceBlock
        {
            get => this.GetDataState(0x008);
            set => this.SetDataState(0x008, value);
        }

        [Cheat(CheatType.MultiDropDown, "Tokens", "Reverse Dice Block")]
        public DataValue TokenReverseDiceBlock
        {
            get => this.GetDataState(0x009);
            set => this.SetDataState(0x009, value);
        }

        [Cheat(CheatType.MultiDropDown, "Tokens", "Slow Dice Block")]
        public DataValue TokenSlowDiceBlock
        {
            get => this.GetDataState(0x00A);
            set => this.SetDataState(0x00A, value);
        }

        [Cheat(CheatType.MultiDropDown, "Tokens", "Double Dice Block")]
        public DataValue TokenDoubleDiceBlock
        {
            get => this.GetDataState(0x00B);
            set => this.SetDataState(0x00B, value);
        }

        [Cheat(CheatType.MultiDropDown, "Tokens", "Bronze")]
        public DataValue TokenBronze
        {
            get => this.GetDataState(0x00C);
            set => this.SetDataState(0x00C, value);
        }

        [Cheat(CheatType.MultiDropDown, "Tokens", "Pow Block")]
        public DataValue TokenPowBlock
        {
            get => this.GetDataState(0x00D);
            set => this.SetDataState(0x00D, value);
        }

        [Cheat(CheatType.MultiDropDown, "Tokens", "Pipe")]
        public DataValue TokenPipe
        {
            get => this.GetDataState(0x00E);
            set => this.SetDataState(0x00E, value);
        }

        [Cheat(CheatType.MultiDropDown, "Tokens", "? Block")]
        public DataValue TokenQuestionBlock
        {
            get => this.GetDataState(0x00F);
            set => this.SetDataState(0x00F, value);
        }

        [Cheat(CheatType.MultiDropDown, "Tokens", "Bowser Space")]
        public DataValue TokenBowserSpace
        {
            get => this.GetDataState(0x010);
            set => this.SetDataState(0x010, value);
        }

        [Cheat(CheatType.MultiDropDown, "Tokens", "Plus 5")]
        public DataValue TokenPlus5
        {
            get => this.GetDataState(0x011);
            set => this.SetDataState(0x011, value);
        }

        [Cheat(CheatType.MultiDropDown, "Tokens", "Silver")]
        public DataValue TokenSilver
        {
            get => this.GetDataState(0x012);
            set => this.SetDataState(0x012, value);
        }

        [Cheat(CheatType.MultiDropDown, "Tokens", "Dash Special")]
        public DataValue TokenDashSpecial
        {
            get => this.GetDataState(0x013);
            set => this.SetDataState(0x013, value);
        }

        [Cheat(CheatType.MultiDropDown, "Tokens", "Reverse Special")]
        public DataValue TokenReverseSpecial
        {
            get => this.GetDataState(0x014);
            set => this.SetDataState(0x014, value);
        }

        [Cheat(CheatType.MultiDropDown, "Tokens", "Jump Special")]
        public DataValue TokenJumpSpecial
        {
            get => this.GetDataState(0x015);
            set => this.SetDataState(0x015, value);
        }

        [Cheat(CheatType.MultiDropDown, "Tokens", "Coin Special")]
        public DataValue TokenCoinSpecial
        {
            get => this.GetDataState(0x016);
            set => this.SetDataState(0x016, value);
        }

        [Cheat(CheatType.MultiDropDown, "Tokens", "Star Special")]
        public DataValue TokenStarSpecial
        {
            get => this.GetDataState(0x017);
            set => this.SetDataState(0x017, value);
        }

        [Cheat(CheatType.MultiDropDown, "Tokens", "Gold")]
        public DataValue TokenGold
        {
            get => this.GetDataState(0x018);
            set => this.SetDataState(0x018, value);
        }

        [Cheat(CheatType.MultiDropDown, "Tokens", "Normal Board")]
        public DataValue TokenNormalBoard
        {
            get => this.GetDataState(0x019);
            set => this.SetDataState(0x019, value);
        }

        [Cheat(CheatType.MultiDropDown, "Tokens", "Mario Board")]
        public DataValue TokenMarioBoard
        {
            get => this.GetDataState(0x01A);
            set => this.SetDataState(0x01A, value);
        }

        [Cheat(CheatType.MultiDropDown, "Tokens", "Luigi Board")]
        public DataValue TokenLuigiBoard
        {
            get => this.GetDataState(0x01B);
            set => this.SetDataState(0x01B, value);
        }

        [Cheat(CheatType.MultiDropDown, "Tokens", "Peach Board")]
        public DataValue TokenPeachBoard
        {
            get => this.GetDataState(0x01C);
            set => this.SetDataState(0x01C, value);
        }

        [Cheat(CheatType.MultiDropDown, "Tokens", "Toad Board")]
        public DataValue TokenToadBoard
        {
            get => this.GetDataState(0x01D);
            set => this.SetDataState(0x01D, value);
        }

        [Cheat(CheatType.MultiDropDown, "Tokens", "Yoshi Board")]
        public DataValue TokenYoshiBoard
        {
            get => this.GetDataState(0x01E);
            set => this.SetDataState(0x01E, value);
        }

        [Cheat(CheatType.MultiDropDown, "Tokens", "Wario Board")]
        public DataValue TokenWarioBoard
        {
            get => this.GetDataState(0x01F);
            set => this.SetDataState(0x01F, value);
        }

        [Cheat(CheatType.MultiDropDown, "Tokens", "Rosalina Board")]
        public DataValue TokenRosalinaBoard
        {
            get => this.GetDataState(0x020);
            set => this.SetDataState(0x020, value);
        }

        [Cheat(CheatType.MultiDropDown, "Tokens", "Donkey Kong Board")]
        public DataValue TokenDonkeyKongBoard
        {
            get => this.GetDataState(0x021);
            set => this.SetDataState(0x021, value);
        }

        [Cheat(CheatType.MultiDropDown, "Tokens", "Bowser Board")]
        public DataValue TokenBowserBoard
        {
            get => this.GetDataState(0x022);
            set => this.SetDataState(0x022, value);
        }

        #endregion

        #region Bases

        [Cheat(CheatType.MultiDropDown, "Bases", "Mushroom")]
        public DataValue BaseMushroom
        {
            get => this.GetDataState(0x02D);
            set => this.SetDataState(0x02D, value);
        }

        [Cheat(CheatType.MultiDropDown, "Bases", "Coin")]
        public DataValue BaseCoin
        {
            get => this.GetDataState(0x02E);
            set => this.SetDataState(0x02E, value);
        }

        [Cheat(CheatType.MultiDropDown, "Bases", "Star")]
        public DataValue BaseStar
        {
            get => this.GetDataState(0x02F);
            set => this.SetDataState(0x02F, value);
        }

        [Cheat(CheatType.MultiDropDown, "Bases", "Sunflower")]
        public DataValue BaseSunflower
        {
            get => this.GetDataState(0x030);
            set => this.SetDataState(0x030, value);
        }

        [Cheat(CheatType.MultiDropDown, "Bases", "Flower")]
        public DataValue BaseFlower
        {
            get => this.GetDataState(0x031);
            set => this.SetDataState(0x031, value);
        }

        [Cheat(CheatType.MultiDropDown, "Bases", "Heart")]
        public DataValue BaseHeart
        {
            get => this.GetDataState(0x032);
            set => this.SetDataState(0x032, value);
        }

        [Cheat(CheatType.MultiDropDown, "Bases", "Orange")]
        public DataValue BaseOrange
        {
            get => this.GetDataState(0x033);
            set => this.SetDataState(0x033, value);
        }

        [Cheat(CheatType.MultiDropDown, "Bases", "Watermelon")]
        public DataValue BaseWatermelon
        {
            get => this.GetDataState(0x034);
            set => this.SetDataState(0x034, value);
        }

        [Cheat(CheatType.MultiDropDown, "Bases", "Kiwi")]
        public DataValue BaseKiwi
        {
            get => this.GetDataState(0x035);
            set => this.SetDataState(0x035, value);
        }

        [Cheat(CheatType.MultiDropDown, "Bases", "Cookie")]
        public DataValue BaseCookie
        {
            get => this.GetDataState(0x036);
            set => this.SetDataState(0x036, value);
        }

        [Cheat(CheatType.MultiDropDown, "Bases", "Doughnut")]
        public DataValue BaseDoughnut
        {
            get => this.GetDataState(0x037);
            set => this.SetDataState(0x037, value);
        }

        [Cheat(CheatType.MultiDropDown, "Bases", "Shortbread")]
        public DataValue BaseShortbread
        {
            get => this.GetDataState(0x038);
            set => this.SetDataState(0x038, value);
        }

        [Cheat(CheatType.MultiDropDown, "Bases", "Football")]
        public DataValue BaseFootball
        {
            get => this.GetDataState(0x039);
            set => this.SetDataState(0x039, value);
        }

        [Cheat(CheatType.MultiDropDown, "Bases", "Baseball")]
        public DataValue BaseBaseball
        {
            get => this.GetDataState(0x03A);
            set => this.SetDataState(0x03A, value);
        }

        [Cheat(CheatType.MultiDropDown, "Bases", "Tyre")]
        public DataValue BaseTyre
        {
            get => this.GetDataState(0x03B);
            set => this.SetDataState(0x03B, value);
        }

        [Cheat(CheatType.MultiDropDown, "Bases", "Bronze")]
        public DataValue BaseBronze
        {
            get => this.GetDataState(0x03C);
            set => this.SetDataState(0x03C, value);
        }

        [Cheat(CheatType.MultiDropDown, "Bases", "Silver")]
        public DataValue BaseSilver
        {
            get => this.GetDataState(0x03D);
            set => this.SetDataState(0x03D, value);
        }

        [Cheat(CheatType.MultiDropDown, "Bases", "Gold")]
        public DataValue BaseGold
        {
            get => this.GetDataState(0x03E);
            set => this.SetDataState(0x03E, value);
        }

        #endregion
    }
}
