using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using BinaryStudio.Diagnostics.Logging;
using BinaryStudio.IO;
using BinaryStudio.Security.Cryptography.AbstractSyntaxNotation;
using BinaryStudio.Security.Cryptography.Certificates;
using BinaryStudio.Security.Cryptography.CryptographicMessageSyntax;
using BinaryStudio.Security.Cryptography.CryptographyServiceProvider.Internal;
using BinaryStudio.Serialization;
using Microsoft.Win32;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    internal class DefaultCryptographicContext : CryptographicObject, ICryptographicContext
        {
        public readonly CryptographicContextFlags flags;
        public override IntPtr Handle { get; }
        protected internal override ILogger Logger { get; }

        public DefaultCryptographicContext(CryptographicContextFlags flags)
            {
            this.flags = flags;
            }

        public IHashAlgorithm CreateHashAlgorithm(Oid algid)
            {
            if (algid == null) { throw new ArgumentNullException(nameof(algid)); }
            var r = CryptoConfig.CreateFromName(algid.Value);
            if (r is IHashAlgorithm i) { return i; }
            if (r is HashAlgorithm engine) { return new HashEngine(engine); }
            throw new NotSupportedException($"Algorithm {{{algid.Value}}} is not supported.");
            }

        public void CreateMessageSignature(Stream inputstream, Stream output, IList<IX509Certificate> certificates, CryptographicMessageFlags flags,RequestSecureStringEventHandler requesthandler)
            {
            throw new NotImplementedException();
            }

        public Boolean VerifyCertificateSignature(out Exception e, IX509Certificate subject, IX509Certificate issuer,CRYPT_VERIFY_CERT_SIGN flags)
            {
            throw new NotImplementedException();
            }

        public void EncryptMessageBER(Oid algid, IList<IX509Certificate> recipients, Stream inputstream, Stream outputstream)
            {
            throw new NotImplementedException();
            }

        public void EncryptMessageDER(Oid algid, IList<IX509Certificate> recipients, Stream inputstream, Stream outputstream)
            {
            throw new NotImplementedException();
            }

        public void VerifyAttachedMessageSignature(Stream input, Stream output, out IList<IX509Certificate> certificates,IX509CertificateResolver finder)
            {
            certificates = EmptyArray<IX509Certificate>.Value;
            if (input == null) { throw new ArgumentNullException(nameof(input)); }
            using (var memory = new MemoryStream()) {
                input.CopyTo(memory);
                using (var ro = new ReadOnlyMemoryMappingStream(memory.ToArray())) {
                    var o = Asn1Object.Load(ro).FirstOrDefault();
                    if (o == null) { throw new ArgumentOutOfRangeException(nameof(input)); }
                    var r = new CmsMessage(o);
                    if (r.IsFailed)  { throw new ArgumentOutOfRangeException(nameof(input)); }
                    if (r.ContentInfo is CmsSignedDataContentInfo ci) {
                        var signerindex = 0;
                        foreach (var si in ci.Signers) {
                            var algid = si.DigestAlgorithm.Identifier.ToString();
                            using (var engine = CreateHashAlgorithm(new Oid(algid))) {
                                var digest = engine.Compute(si.DigestValueSource);
                                Console.WriteLine("SIGNER_{0}:CMSG_COMPUTED_HASH_PARAM:{1}", signerindex, String.Join(String.Empty, digest.ToString("X")));
                                signerindex++;
                                }
                            }
                        }
                    }
                }
            }

        public void VerifyDetachedMessageSignature(Stream input, Stream inputdata, out IList<IX509Certificate> certificates,IX509CertificateResolver finder)
            {
            throw new NotImplementedException();
            }

        private class HashEngine : HashAlgorithm, IHashAlgorithm
            {
            private HashAlgorithm UnderlyingObject;
            public HashEngine(HashAlgorithm source)
                {
                if (source == null) { throw new ArgumentNullException(nameof(source)); }
                UnderlyingObject = source;
                }

            /// <summary>Initializes an implementation of the <see cref="T:System.Security.Cryptography.HashAlgorithm" /> class.</summary>
            public override void Initialize()
                {
                UnderlyingObject.Initialize();
                }

            Byte[] IHashAlgorithm.Compute(Stream inputstream)
                {
                return UnderlyingObject.ComputeHash(inputstream);
                }

            Byte[] IHashAlgorithm.Compute(Byte[] bytes)
                {
                return UnderlyingObject.ComputeHash(bytes);
                }

            void IHashAlgorithm.CreateSignature(Stream signature, KeySpec keyspec)
                {
                throw new NotImplementedException();
                }

            /// <summary>When overridden in a derived class, routes data written to the object into the hash algorithm for computing the hash.</summary>
            /// <param name="buffer">The input to compute the hash code for.</param>
            /// <param name="offset">The offset into the byte array from which to begin using data.</param>
            /// <param name="length">The number of bytes in the byte array to use as data.</param>
            protected override void HashCore(Byte[] buffer, Int32 offset, Int32 length)
                {
                UnderlyingObject.TransformBlock(buffer, offset, length, null, 0);
                }

            /// <summary>When overridden in a derived class, finalizes the hash computation after the last data is processed by the cryptographic stream object.</summary>
            /// <returns>The computed hash code.</returns>
            protected override Byte[] HashFinal()
                {
                return UnderlyingObject.TransformFinalBlock(EmptyArray<Byte>.Value, 0, 0);
                }

            /// <summary>Releases the unmanaged resources used by the <see cref="T:System.Security.Cryptography.HashAlgorithm" /> and optionally releases the managed resources.</summary>
            /// <param name="disposing">
            /// <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
            protected override void Dispose(Boolean disposing) {
                if (disposing) {
                    if (UnderlyingObject != null) {
                        UnderlyingObject.Dispose();
                        UnderlyingObject = null;
                        }
                    }
                base.Dispose(disposing);
                }
            }
        }
    }