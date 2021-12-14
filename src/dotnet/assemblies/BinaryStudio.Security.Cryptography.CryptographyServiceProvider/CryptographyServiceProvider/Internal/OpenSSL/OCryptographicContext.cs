using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using BinaryStudio.Diagnostics.Logging;
using BinaryStudio.Security.Cryptography.Certificates;
using Microsoft.Win32;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider.OpenSSL
    {
    internal class OCryptographicContext: CryptographicObject, ICryptographicContext
        {
        public override IntPtr Handle { get; }
        protected internal override ILogger Logger { get; }
        public IHashAlgorithm CreateHashAlgorithm(Oid algid)
            {
            throw new NotImplementedException();
            }

        public void CreateMessageSignature(Stream inputstream, Stream output, IList<IX509Certificate> certificates, CryptographicMessageFlags flags,RequestSecureStringEventHandler requesthandler)
            {
            throw new NotImplementedException();
            }

        public Boolean VerifyCertificateSignature(out Exception e, IX509Certificate subject, IX509Certificate issuer, CRYPT_VERIFY_CERT_SIGN flags)
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
        }
    }