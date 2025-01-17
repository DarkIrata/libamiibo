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

using System.Text;

namespace LibAmiibo.Helper
{
    //https://wiki.gbatemp.net/wiki/Amiibo
    public static class NtagHelper
    {
        public const int NFC3D_AMIIBO_SIZE = 552;
        public const int NFC3D_NTAG_SIZE = 572;

        public static readonly byte[] NTAG_PUB_KEY =
        {
            0x04, 0x49, 0x4E, 0x1A,
            0x38, 0x6D, 0x3D, 0x3C,
            0xFE, 0x3D, 0xC1, 0x0E,
            0x5D, 0xE6, 0x8A, 0x49,
            0x9B, 0x1C, 0x20, 0x2D,
            0xB5, 0xB1, 0x32, 0x39,
            0x3E, 0x89, 0xED, 0x19,
            0xFE, 0x5B, 0xE8, 0xBC,
            0x61
        };

        public static readonly byte[] CONFIG_BYTES =
        {
            0x01, 0x00, 0x0F, 0xBD,     // Dynamic lock bytes + RFUI
            0x00, 0x00, 0x00, 0x04,     // CFG0
            0x5F, 0x00, 0x00, 0x00      // CFG1
        };


        public static byte[] GetInternalTag(byte[] tag)
        {
            var internalBytes = new byte[NFC3D_AMIIBO_SIZE];
            TagToInternal(tag, internalBytes);

            return internalBytes;
        }

        public static void TagToInternal(byte[] tag, byte[] intern)
        {
            // 0x02C - 0x1B3 Crypto buffer
            Array.Copy(tag, 0x008, intern, 0x000, 0x008);     // LockBytes + CC
            Array.Copy(tag, 0x080, intern, 0x008, 0x020);     // Data Signature (signs 0x029 - 0x208)
            Array.Copy(tag, 0x010, intern, 0x028, 0x024);     // 0x010 - 0x013 unencrypted, 0x014 begin of encrypted section
            Array.Copy(tag, 0x0A0, intern, 0x04C, 0x168);     // Encrypted data buffer
            Array.Copy(tag, 0x034, intern, 0x1B4, 0x020);     // Tag Signature (signs 0x1D4 - 0x208)
            Array.Copy(tag, 0x000, intern, 0x1D4, 0x008);     // NTAG Serial
            Array.Copy(tag, 0x054, intern, 0x1DC, 0x02C);     // Plaintext data

            // ECDSA of tag:
            if (tag.Length == NFC3D_NTAG_SIZE)
            {
                Array.Copy(tag, 0x21C, intern, 0x208, 0x020);
            }
            else
            {
                for (int i = 0x208; i < 0x208 + 0x20; i++)
                {
                    intern[i] = 0xFF;
                }
            }
        }

        public static void InternalToTag(byte[] intl, byte[] tag)
        {
            Array.Copy(intl, 0x000, tag, 0x008, 0x008);
            Array.Copy(intl, 0x008, tag, 0x080, 0x020);
            Array.Copy(intl, 0x028, tag, 0x010, 0x024);
            Array.Copy(intl, 0x04C, tag, 0x0A0, 0x168);
            Array.Copy(intl, 0x1B4, tag, 0x034, 0x020);
            Array.Copy(intl, 0x1D4, tag, 0x000, 0x008);
            Array.Copy(intl, 0x1DC, tag, 0x054, 0x02C);
            Array.Copy(intl, 0x208, tag, 0x21C, 0x020);
            Array.Copy(CONFIG_BYTES, 0x000, tag, 0x208, 0x00C);
        }

        public static ushort UInt16FromTag(ArraySegment<byte> buffer, int offset, bool useLittleEndian = false)
        {
            var data = new byte[0x02];
            Array.Copy(buffer.Array, buffer.Offset + offset, data, 0, data.Length);
            ApplyByteOrder(data, useLittleEndian);
            return BitConverter.ToUInt16(data, 0);
        }

        public static void UInt16ToTag(ArraySegment<byte> buffer, int offset, ushort value, bool useLittleEndian = false)
        {
            var data = BitConverter.GetBytes(value);
            ApplyByteOrder(data, useLittleEndian);
            buffer.CopyFrom(data, 0, offset, data.Length);
        }

        public static uint UInt24FromTag(ArraySegment<byte> buffer, int offset, bool useLittleEndian = false)
        {
            var useLE = useLittleEndian ^ BitConverter.IsLittleEndian;
            var data = new byte[0x04];
            Array.Copy(buffer.Array, buffer.Offset + offset, data, useLE ? 1 : 0, data.Length);
            ApplyByteOrder(data, useLittleEndian);
            return BitConverter.ToUInt32(data, 0);
        }

        public static void UInt24ToTag(ArraySegment<byte> buffer, int offset, uint value, bool useLittleEndian = false)
        {
            var useLE = useLittleEndian ^ BitConverter.IsLittleEndian;
            var data = BitConverter.GetBytes(value);
            ApplyByteOrder(data, useLittleEndian);
            buffer.CopyFrom(data, useLE ? 1 : 0, offset, 3);
        }

        public static uint UInt32FromTag(ArraySegment<byte> buffer, int offset, bool useLittleEndian = false)
        {
            var data = new byte[0x04];
            Array.Copy(buffer.Array, buffer.Offset + offset, data, 0, data.Length);
            ApplyByteOrder(data, useLittleEndian);
            return BitConverter.ToUInt32(data, 0);
        }

        public static void UInt32ToTag(ArraySegment<byte> buffer, int offset, uint value, bool useLittleEndian = false)
        {
            var data = BitConverter.GetBytes(value);
            ApplyByteOrder(data, useLittleEndian);
            buffer.CopyFrom(data, 0, offset, data.Length);
        }

        public static ulong UInt64FromTag(ArraySegment<byte> buffer, int offset, bool useLittleEndian = false)
        {
            var data = new byte[0x08];
            Array.Copy(buffer.Array, buffer.Offset + offset, data, 0, data.Length);
            ApplyByteOrder(data, useLittleEndian);
            return BitConverter.ToUInt64(data, 0);
        }

        public static void UInt64ToTag(ArraySegment<byte> buffer, int offset, ulong value, bool useLittleEndian = false)
        {
            var data = BitConverter.GetBytes(value);
            ApplyByteOrder(data, useLittleEndian);
            buffer.CopyFrom(data, 0, offset, data.Length);
        }

        public static DateTime DateTimeFromTag(ushort value)
        {
            var day = value & 0x1F;
            var month = (value >> 5) & 0x0F;
            var year = (value >> 9) & 0x7F;
            return new DateTime(2000 + year, month, day);
        }

        public static ushort DateTimeToTag(DateTime value)
        {
            int result = 0;
            result |= value.Year - 2000;
            result <<= 4;
            result |= value.Month;
            result <<= 5;
            result |= value.Day;
            return (ushort)result;
        }

        public static void ApplyByteOrder(byte[] data, bool forceUseLittleEndian = false)
        {
            if (forceUseLittleEndian ^ BitConverter.IsLittleEndian)
            {
                Array.Reverse(data);
            }
        }

        public static string ByteArrayToString(byte[] bytes)
        {
            var hex = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes)
            {
                hex.AppendFormat("{0:x2}", b);
            }

            return hex.ToString();
        }

        public static byte[] StringToByteArrayFastest(string hex)
        {
            if (hex.Length % 2 != 0)
            {
                throw new Exception("The binary key cannot have an odd number of digits");
            }

            byte[] arr = new byte[hex.Length >> 1];
            for (int i = 0; i < hex.Length >> 1; ++i)
            {
                arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
            }

            return arr;
        }

        private static int GetHexVal(char hex)
        {
            int val = (int)hex;
            return val - (val < 58 ? 48 : val < 71 ? 55 : 87);
        }
    }
}
