﻿
using System;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    [AlgorithmIdentifer(ObjectIdentifiers.szOID_OIWSEC_sha1)]
    internal class OpenSSLSHA1 : OpenSSLHashAlgorithm
        {
        private SHA_CTX context;
        private Boolean ready;

        /// <inheritdoc/>
        public OpenSSLSHA1(OpenSSLCryptographicContext context)
            : base(context)
            {
            HashSizeValue = 160;
            }

        private void Ensure() {
            if (!ready) {
                Context.Core.SHA1_Init(ref context);
                ready = true;
                }
            }

        /// <summary>Initializes an implementation of the <see cref="T:System.Security.Cryptography.HashAlgorithm" /> class.</summary>
        public override void Initialize()
            {
            ready = false;
            }

        /// <summary>When overridden in a derived class, routes data written to the object into the hash algorithm for computing the hash.</summary>
        /// <param name="data">The input to compute the hash code for.</param>
        /// <param name="firstindex">The offset into the byte array from which to begin using data.</param>
        /// <param name="length">The number of bytes in the byte array to use as data.</param>
        protected override unsafe void HashCore(Byte[] data, Int32 firstindex, Int32 length) {
            Ensure();
            fixed (Byte* r = data) {
                Context.Core.SHA1_Update(ref context, r + firstindex, (IntPtr)length);
                }
            }

        /// <summary>When overridden in a derived class, finalizes the hash computation after the last data is processed by the cryptographic stream object.</summary>
        /// <returns>The computed hash code.</returns>
        protected override Byte[] HashFinal()
            {
            Ensure();
            var r = new Byte[HashSizeValue >> 3];
            Context.Core.SHA1_Final(r, ref context);
            return r;
            }
        }
    }