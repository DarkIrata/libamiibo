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

using System.ComponentModel;
using LibAmiibo.Attributes;
using LibAmiibo.Helper;

namespace LibAmiibo.Data.AppData.Games
{
    [AppID(0x10199000)]
    [AppDataInitializationTitleID("00050000101A3600")]
    [AppDataInitializationTitleID("0005000010199000")]
    [AppDataInitializationTitleID("00050000101A3500")]
    public class MarioTennis : IGame
    {
        private ArraySegment<byte> AppData { get; set; }

        public enum StatusTrainingValue
        {
            None,
            [Description("Stroke (max. 3 slots)")]
            Stroke,
            [Description("Serve (max. 2 slots)")]
            Serve,
            [Description("Acceleration (max. 2 slots)")]
            Acceleration,
            [Description("Speed (max. 3 slots)")]
            Speed,
            [Description("Curve (max. 3 slots)")]
            Curve,
            [Description("Agility (max. 4 slots)")]
            Agility,
            [Description("Control (max. 2 slots)")]
            Control,
            [Description("Skill (max. 3 slots)")]
            Skill,
        }

        private StatusTrainingValue GetStatusTrainingSlot(int slotId)
        {
            var offset = this.AppData.Offset + 0x002 + slotId;
            return (StatusTrainingValue)this.AppData.Array[offset];
        }

        private void SetStatusTrainingSlot(int slotId, StatusTrainingValue value)
        {
            var offset = this.AppData.Offset + 0x002 + slotId;
            this.AppData.Array[offset] = (byte)value;
        }

        public MarioTennis(ArraySegment<byte> appData)
        {
            this.AppData = appData;
        }

        [SupportedGame(typeof(MarioTennis))]
        public class Initializer : IAppDataInitializer
        {
            public void InitializeAppData(AmiiboTag tag)
            {
                this.ThrowOnInvalidAppId(tag);
                var game = new MarioTennis(tag.AppDataBuffer);
                game.AppData.CopyFrom(new byte[0x0B]); // TODO: Use for-loop and create extension method
            }
        }

        #region Misc

        [Cheat(CheatType.NumberSpinner, "Misc", "Games played")]
        public ushort MatchCount
        {
            get => NtagHelper.UInt16FromTag(this.AppData, 0x00);
            set => NtagHelper.UInt16ToTag(this.AppData, 0x00, value);
        }

        #endregion

        #region Stats

        [Cheat(CheatType.DropDown, "Stats", "Slot 1")]
        public StatusTrainingValue StatusTrainingSlot1
        {
            get => this.GetStatusTrainingSlot(0);
            set => this.SetStatusTrainingSlot(0, value);
        }

        [Cheat(CheatType.DropDown, "Stats", "Slot 2")]
        public StatusTrainingValue StatusTrainingSlot2
        {
            get => this.GetStatusTrainingSlot(1);
            set => this.SetStatusTrainingSlot(1, value);
        }

        [Cheat(CheatType.DropDown, "Stats", "Slot 3")]
        public StatusTrainingValue StatusTrainingSlot3
        {
            get => this.GetStatusTrainingSlot(2);
            set => this.SetStatusTrainingSlot(2, value);
        }

        [Cheat(CheatType.DropDown, "Stats", "Slot 4")]
        public StatusTrainingValue StatusTrainingSlot4
        {
            get => this.GetStatusTrainingSlot(3);
            set => this.SetStatusTrainingSlot(3, value);
        }

        [Cheat(CheatType.DropDown, "Stats", "Slot 5")]
        public StatusTrainingValue StatusTrainingSlot5
        {
            get => this.GetStatusTrainingSlot(4);
            set => this.SetStatusTrainingSlot(4, value);
        }

        [Cheat(CheatType.DropDown, "Stats", "Slot 6")]
        public StatusTrainingValue StatusTrainingSlot6
        {
            get => this.GetStatusTrainingSlot(5);
            set => this.SetStatusTrainingSlot(5, value);
        }

        [Cheat(CheatType.DropDown, "Stats", "Slot 7")]
        public StatusTrainingValue StatusTrainingSlot7
        {
            get => this.GetStatusTrainingSlot(6);
            set => this.SetStatusTrainingSlot(6, value);
        }

        [Cheat(CheatType.DropDown, "Stats", "Slot 8")]
        public StatusTrainingValue StatusTrainingSlot8
        {
            get => this.GetStatusTrainingSlot(7);
            set => this.SetStatusTrainingSlot(7, value);
        }

        [Cheat(CheatType.DropDown, "Stats", "Slot 9")]
        public StatusTrainingValue StatusTrainingSlot9
        {
            get => this.GetStatusTrainingSlot(8);
            set => this.SetStatusTrainingSlot(8, value);
        }

        [Cheat(CheatType.DropDown, "Stats", "Slot 10")]
        public StatusTrainingValue StatusTrainingSlot10
        {
            get => this.GetStatusTrainingSlot(9);
            set => this.SetStatusTrainingSlot(9, value);
        }

        #endregion
    }
}
