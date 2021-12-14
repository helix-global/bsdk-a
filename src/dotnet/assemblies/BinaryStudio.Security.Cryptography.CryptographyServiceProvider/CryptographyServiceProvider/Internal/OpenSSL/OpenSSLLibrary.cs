using System;
using BinaryStudio.PlatformComponents;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    internal class OpenSSLLibrary : Library, IOpenSSLLibrary
        {
        public OpenSSLLibrary(String filepath)
            : base(filepath)
            {
            }

        Int32 IOpenSSLLibrary.SHA256_Init(ref SHA256_CTX c)
            {
            return EnsureProcedure("SHA256_Init", ref FSHA256_Init)(ref c);
            }

        Int32 IOpenSSLLibrary.SHA256_Final(Byte[] digest, ref SHA256_CTX c)
            {
            return EnsureProcedure("SHA256_Final", ref FSHA256_Final)(digest, ref c);
            }

        public unsafe Int32 SHA256_Update(ref SHA256_CTX c, Byte* data, IntPtr length)
            {
            return EnsureProcedure("SHA256_Update", ref FSHA256_Update)(ref c, data, length);
            }

        Int32 IOpenSSLLibrary.SHA224_Init(ref SHA256_CTX c)
            {
            return EnsureProcedure("SHA224_Init", ref FSHA224_Init)(ref c);
            }

        Int32 IOpenSSLLibrary.SHA224_Final(Byte[] digest, ref SHA256_CTX c)
            {
            return EnsureProcedure("SHA224_Final", ref FSHA224_Final)(digest, ref c);
            }

        public unsafe Int32 SHA224_Update(ref SHA256_CTX c, Byte* data, IntPtr length)
            {
            return EnsureProcedure("SHA224_Update", ref FSHA224_Update)(ref c, data, length);
            }

        private delegate Int32 DSHA256_Init(ref SHA256_CTX c);
        private unsafe delegate Int32 DSHA256_Update(ref SHA256_CTX c, Byte* data, IntPtr length);
        private delegate Int32 DSHA256_Final(Byte[] digest, ref SHA256_CTX c);
        private DSHA256_Init   FSHA256_Init;
        private DSHA256_Update FSHA256_Update;
        private DSHA256_Final  FSHA256_Final;
        private DSHA256_Init   FSHA224_Init;
        private DSHA256_Update FSHA224_Update;
        private DSHA256_Final  FSHA224_Final;
        }
    }