
using System;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    [AlgorithmIdentifer(ObjectIdentifiers.szOID_NIST_sha256)]
    internal class OpenSSLSHA256 : OpenSSLHashAlgorithm
        {
        /// <inheritdoc/>
        public OpenSSLSHA256(OpenSSLCryptographicContext context)
            : base(context)
            {
            }

        /// <summary>Initializes an implementation of the <see cref="T:System.Security.Cryptography.HashAlgorithm" /> class.</summary>
        public override void Initialize()
            {
            throw new NotImplementedException();
            }

        /// <summary>When overridden in a derived class, routes data written to the object into the hash algorithm for computing the hash.</summary>
        /// <param name="array">The input to compute the hash code for.</param>
        /// <param name="ibStart">The offset into the byte array from which to begin using data.</param>
        /// <param name="cbSize">The number of bytes in the byte array to use as data.</param>
        protected override void HashCore(Byte[] array, Int32 ibStart, Int32 cbSize)
            {
            throw new NotImplementedException();
            }

        /// <summary>When overridden in a derived class, finalizes the hash computation after the last data is processed by the cryptographic stream object.</summary>
        /// <returns>The computed hash code.</returns>
        protected override Byte[] HashFinal()
            {
            throw new NotImplementedException();
            }
        }
    }