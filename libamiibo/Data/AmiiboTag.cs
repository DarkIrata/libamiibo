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
 *The above copyright notice and this permission notice shall be included in
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

using LibAmiibo.Data.AppData.Games;
using LibAmiibo.Helper;
using LibAmiibo.Interfaces;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;

namespace LibAmiibo.Data
{
    public class AmiiboTag
    {
        public ArraySegment<byte> Data { get; }

        public bool IsDecrypted { get; internal set; }

        public AmiiboSettings Settings { get; }

        private Amiibo amiibo;

        public Amiibo Amiibo
        {
            get => this.amiibo;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                this.amiibo = value;
                var amiiboBuffer = new ArraySegment<byte>(this.Data.Array, 0x1DC, 0x08);
                amiiboBuffer.CopyFrom(NtagHelper.StringToByteArrayFastest(value.StatueId));
            }
        }

        /// <summary>
        /// Only valid if HasAppData returns true. Use AmiiboSettings.AmiiboAppData for easier access.
        /// </summary>
        public ArraySegment<byte> AppDataBuffer
        {
            get => new ArraySegment<byte>(this.Data.Array, 0x0DC, 0xD8);
            set => this.AppDataBuffer.CopyFrom(value);
        }

        public ArraySegment<byte> NtagSerial
        {
            get => new ArraySegment<byte>(this.Data.Array, 0x1D4, 0x008);
            set => this.NtagSerial.CopyFrom(value);
        }

        public ArraySegment<byte> LockBytesCC
        {
            get => new ArraySegment<byte>(this.Data.Array, 0x000, 0x008);
            set => this.LockBytesCC.CopyFrom(value);
        }

        public ArraySegment<byte> DataSignature
        {
            get => new ArraySegment<byte>(this.Data.Array, 0x008, 0x20);
            set => this.DataSignature.CopyFrom(value);
        }

        /// <summary>
        /// Signs Internal Tag Data from 0x1D4 to 0x208.
        /// </summary>
        public ArraySegment<byte> TagSignature
        {
            get => new ArraySegment<byte>(this.Data.Array, 0x1B4, 0x20);
            set => this.TagSignature.CopyFrom(value);
        }

        public ArraySegment<byte> CryptoInitSequence
        {
            get => new ArraySegment<byte>(this.Data.Array, 0x028, 0x004);
            set => this.CryptoInitSequence.CopyFrom(value);
        }

        public ArraySegment<byte> CryptoBuffer
        {
            get => new ArraySegment<byte>(this.Data.Array, 0x02C, 0x188);
            set => this.CryptoBuffer.CopyFrom(value);
        }

        public ArraySegment<byte> PlaintextData
        {
            get => new ArraySegment<byte>(this.Data.Array, 0x1DC, 0x02C);
            set => this.PlaintextData.CopyFrom(value);
        }

        public ArraySegment<byte> NtagECDSASignature
        {
            get => new ArraySegment<byte>(this.Data.Array, 0x208, 0x020);
            set => this.NtagECDSASignature.CopyFrom(value);
        }

        public byte[] UID
        {
            get
            {
                IList<byte> ntagSerial = this.NtagSerial;
                return new[] { ntagSerial[0], ntagSerial[1], ntagSerial[2], ntagSerial[4], ntagSerial[5], ntagSerial[6], ntagSerial[7] };
            }

            set
            {
                var bcc0 = (byte)(0x88 ^ value[0] ^ value[1] ^ value[2]);
                var bcc1 = (byte)(value[3] ^ value[4] ^ value[5] ^ value[6]);
                this.NtagSerial.CopyFrom(new[] { value[0], value[1], value[2], bcc0, value[3], value[4], value[5], value[6] });
                this.LockBytesCC.CopyFrom(new[] { bcc1 });
            }
        }

        public ushort WriteCounter
        {
            get => NtagHelper.UInt16FromTag(this.Data, 0x029);
            set => NtagHelper.UInt16ToTag(this.Data, 0x029, value);
        }

        public bool HasAppData => this.IsDecrypted && this.Settings.Status.HasFlag(Status.AppDataInitialized);

        public bool HasUserData => this.IsDecrypted && this.Settings.Status.HasFlag(Status.UserDataInitialized);

        public AmiiboTag(ArraySegment<byte> data, IAmiiboInfoDataProvider infoDataProvider)
        {
            this.Data = data;
            this.Amiibo = Amiibo.FromInternalTag(data, infoDataProvider);
            this.Settings = new AmiiboSettings(this.CryptoBuffer, this.AppDataBuffer);
        }

        public void RandomizeUID()
        {
            var rand = new Random();
            var bytes = new byte[7];
            rand.NextBytes(bytes);
            bytes[0] = 0x04; // NXP manufacturer code
            if (bytes[4] == 0x88)
            {
                bytes[4]++; // This must not be 0x88. Just increasing one to avoid the case when another random would result in 0x88
            }

            this.UID = bytes;
        }

        public bool IsUidValid()
        {
            var uid = this.UID;
            var bcc0In = this.NtagSerial.Skip(3).First();
            var bcc1In = this.LockBytesCC.First();
            var bcc0Out = (byte)(0x88 ^ uid[0] ^ uid[1] ^ uid[2]);
            var bcc1Out = (byte)(uid[3] ^ uid[4] ^ uid[5] ^ uid[6]);
            return bcc0In == bcc0Out && bcc1In == bcc1Out & uid[0] == 0x04 && uid[4] != 0x88;
        }

        public bool IsNtagECDSASignatureValid()
        {
            var uid = this.UID;
            if (this.NtagECDSASignature.Count != 0x20 || uid.Length != 7)
            {
                return false;
            }

            var r = new byte[this.NtagECDSASignature.Count / 2];
            var s = new byte[this.NtagECDSASignature.Count / 2];
            Array.Copy(this.NtagECDSASignature.Array, this.NtagECDSASignature.Offset, r, 0, r.Length);
            Array.Copy(this.NtagECDSASignature.Array, this.NtagECDSASignature.Offset + r.Length, s, 0, s.Length);

            var curve = SecNamedCurves.GetByName("secp128r1");
            var curveSpec = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H, curve.GetSeed());
            var key = new ECPublicKeyParameters("ECDSA", curve.Curve.DecodePoint(NtagHelper.NTAG_PUB_KEY), curveSpec);
            var signer = SignerUtilities.GetSigner("NONEwithECDSA");

            signer.Init(false, key);

            signer.BlockUpdate(uid, 0, uid.Length);

            using (var ms = new MemoryStream())
            using (var der = Asn1OutputStream.Create(ms))
            {
                var v = new Asn1EncodableVector
                {
                    new DerInteger(new BigInteger(1, r)),
                    new DerInteger(new BigInteger(1, s))
                };
                der.WriteObject(new DerSequence(v));

                return signer.VerifySignature(ms.ToArray());
            }
        }

        public void InitializeAppData<T>()
            where T : IAppDataInitializer, new()
        {
            var factory = new T();
            var settings = this.Settings;
            var appData = settings.AppData;

            appData.InitializationTitleID = factory.GetInitializationTitleIDs().First();
            appData.AppID = factory.GetAppID() ?? throw new InvalidOperationException("The AppID was not found");
            settings.Status |= Status.AppDataInitialized;
            settings.LastModifiedDate = DateTime.UtcNow.Date;
            settings.WriteCounter++;

            factory.InitializeAppData(this);
            this.WriteCounter++;
        }
    }
}
