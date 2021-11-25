using System;
using System.Runtime.InteropServices;

namespace Microsoft.Win32
    {
    [StructLayout(LayoutKind.Sequential)]
    public struct CMSG_SIGNER_ENCODE_INFO32
        {
        public Int32 Size;
        public unsafe CERT_INFO* CertInfo;
        public IntPtr CryptProvOrKey;
        public KEY_SPEC_TYPE KeySpec;
        public CRYPT_ALGORITHM_IDENTIFIER HashAlgorithm;
        public unsafe void* pvHashAuxInfo;
        public Int32 cAuthAttr;
        public unsafe CRYPT_ATTRIBUTE* rgAuthAttr;
        public Int32 cUnauthAttr;
        public unsafe CRYPT_ATTRIBUTE* rgUnauthAttr;
        #if CMSG_SIGNER_ENCODE_INFO_HAS_CMS_FIELDS
        public CERT_ID32 SignerId;
        public CRYPT_ALGORITHM_IDENTIFIER HashEncryptionAlgorithm;
        public IntPtr pvHashEncryptionAuxInfo;
        #endif
        }

    [StructLayout(LayoutKind.Sequential)]
    public struct CMSG_SIGNER_ENCODE_INFO64
        {
        public Int32 Size;
        public unsafe CERT_INFO* CertInfo;
        public IntPtr CryptProvOrKey;
        public KEY_SPEC_TYPE KeySpec;
        public CRYPT_ALGORITHM_IDENTIFIER HashAlgorithm;
        public unsafe void* pvHashAuxInfo;
        public Int32 cAuthAttr;
        public unsafe CRYPT_ATTRIBUTE* rgAuthAttr;
        public Int32 cUnauthAttr;
        public unsafe CRYPT_ATTRIBUTE* rgUnauthAttr;
        #if CMSG_SIGNER_ENCODE_INFO_HAS_CMS_FIELDS
        public CERT_ID64 SignerId;
        public CRYPT_ALGORITHM_IDENTIFIER HashEncryptionAlgorithm;
        public IntPtr pvHashEncryptionAuxInfo;
        #endif
        }
    }