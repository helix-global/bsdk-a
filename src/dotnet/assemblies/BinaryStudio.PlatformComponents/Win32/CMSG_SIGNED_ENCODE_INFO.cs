using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32
    {
    using CERT_BLOB = CRYPT_BLOB;
    using CRL_BLOB  = CRYPT_BLOB;

    [StructLayout(LayoutKind.Sequential)]
    public struct CMSG_SIGNED_ENCODE_INFO32
        {
        public Int32 Size;
        public Int32 SignerCount;
        public unsafe CMSG_SIGNER_ENCODE_INFO32* Signers;
        public Int32 CertificateCount;
        public unsafe CERT_BLOB* Certificates;
        public Int32 cCrlEncoded;
        public unsafe CRL_BLOB* rgCrlEncoded;
        #if CMSG_SIGNED_ENCODE_INFO_HAS_CMS_FIELDS
        DWORD       cAttrCertEncoded;
        PCERT_BLOB  rgAttrCertEncoded;
        #endif
        }

    [StructLayout(LayoutKind.Sequential)]
    public struct CMSG_SIGNED_ENCODE_INFO64
        {
        public Int32 Size;
        public Int32 SignerCount;
        public unsafe CMSG_SIGNER_ENCODE_INFO64* Signers;
        public Int32 CertificateCount;
        public unsafe CERT_BLOB* Certificates;
        public Int32 cCrlEncoded;
        public unsafe CRL_BLOB* rgCrlEncoded;
        #if CMSG_SIGNED_ENCODE_INFO_HAS_CMS_FIELDS
        DWORD       cAttrCertEncoded;
        PCERT_BLOB  rgAttrCertEncoded;
        #endif
        }
    }