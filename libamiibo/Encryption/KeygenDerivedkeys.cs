﻿/*
 * Copyright (C) 2015 Marcos Vives Del Sol
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
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace LibAmiibo.Encryption
{
    public class KeygenDerivedKeys
    {
        public byte[] aesKey;  // 16 bytes
        public byte[] aesIV;   // 16 bytes
        public byte[] hmacKey; // 16 bytes

        internal static KeygenDerivedKeys Unserialize(BinaryReader reader) => new KeygenDerivedKeys
        {
            aesKey = reader.ReadBytes(16),
            aesIV = reader.ReadBytes(16),
            hmacKey = reader.ReadBytes(16)
        };

        internal void Serialize(BinaryWriter writer)
        {
            writer.Write(this.aesKey);
            writer.Write(this.aesIV);
            writer.Write(this.hmacKey);
        }

        protected bool Equals(KeygenDerivedKeys other) => NativeHelpers.MemCmp(this.aesKey, other.aesKey, 0, this.aesKey.Length) &&
                NativeHelpers.MemCmp(this.aesIV, other.aesIV, 0, this.aesIV.Length) &&
                NativeHelpers.MemCmp(this.hmacKey, other.hmacKey, 0, this.hmacKey.Length);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((KeygenDerivedKeys)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (this.aesKey != null ? this.aesKey.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.aesIV != null ? this.aesIV.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.hmacKey != null ? this.hmacKey.GetHashCode() : 0);
                return hashCode;
            }
        }

        public void Cipher(byte[] input, byte[] output, bool forEncryption)
        {
            var cipher = CipherUtilities.GetCipher("AES/CTR/NoPadding");
            ParametersWithIV ivAndKey = new ParametersWithIV(new KeyParameter(this.aesKey), this.aesIV);
            cipher.Init(forEncryption, ivAndKey);
            var pos = cipher.ProcessBytes(input, 0x02C, 0x188, output, 0x02C);
            cipher.DoFinal(output, 0x02C + pos);

            Array.Copy(input, 0x000, output, 0x000, 0x008);
            // Data signature NOT copied
            Array.Copy(input, 0x028, output, 0x028, 0x004);
            // Tag signature NOT copied
            Array.Copy(input, 0x1D4, output, 0x1D4, 0x054);
        }
    }
}