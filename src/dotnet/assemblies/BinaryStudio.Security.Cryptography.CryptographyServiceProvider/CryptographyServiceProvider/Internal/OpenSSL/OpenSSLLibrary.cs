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

        public Int32 SHA1_Init(ref SHA_CTX c)
            {
            return EnsureProcedure("SHA1_Init", ref FSHA1_Init)(ref c);
            }

        Int32 IOpenSSLLibrary.SHA256_Init(ref SHA256_CTX c)
            {
            return EnsureProcedure("SHA256_Init", ref FSHA256_Init)(ref c);
            }

        public Int32 SHA512_Init(ref SHA512_CTX c)
            {
            return EnsureProcedure("SHA512_Init", ref FSHA512_Init)(ref c);
            }

        public Int32 SHA1_Final(Byte[] digest, ref SHA_CTX c)
            {
            return EnsureProcedure("SHA1_Final", ref FSHA1_Final)(digest, ref c);
            }

        Int32 IOpenSSLLibrary.SHA256_Final(Byte[] digest, ref SHA256_CTX c)
            {
            return EnsureProcedure("SHA256_Final", ref FSHA256_Final)(digest, ref c);
            }

        public Int32 SHA512_Final(Byte[] digest, ref SHA512_CTX c)
            {
            return EnsureProcedure("SHA512_Final", ref FSHA512_Final)(digest, ref c);
            }

        public unsafe Int32 SHA1_Update(ref SHA_CTX c, Byte* data, IntPtr length)
            {
            return EnsureProcedure("SHA1_Update", ref FSHA1_Update)(ref c, data, length);
            }

        public unsafe Int32 SHA256_Update(ref SHA256_CTX c, Byte* data, IntPtr length)
            {
            return EnsureProcedure("SHA256_Update", ref FSHA256_Update)(ref c, data, length);
            }

        Int32 IOpenSSLLibrary.SHA224_Init(ref SHA256_CTX c)
            {
            return EnsureProcedure("SHA224_Init", ref FSHA224_Init)(ref c);
            }

        public Int32 SHA384_Init(ref SHA512_CTX c)
            {
            return EnsureProcedure("SHA384_Init", ref FSHA384_Init)(ref c);
            }

        Int32 IOpenSSLLibrary.SHA224_Final(Byte[] digest, ref SHA256_CTX c)
            {
            return EnsureProcedure("SHA224_Final", ref FSHA224_Final)(digest, ref c);
            }

        public Int32 SHA384_Final(Byte[] digest, ref SHA512_CTX c)
            {
            return EnsureProcedure("SHA384_Final", ref FSHA384_Final)(digest, ref c);
            }

        public unsafe Int32 SHA224_Update(ref SHA256_CTX c, Byte* data, IntPtr length)
            {
            return EnsureProcedure("SHA224_Update", ref FSHA224_Update)(ref c, data, length);
            }

        public unsafe Int32 SHA384_Update(ref SHA512_CTX c, Byte* data, IntPtr length)
            {
            return EnsureProcedure("SHA384_Update", ref FSHA384_Update)(ref c, data, length);
            }

        public unsafe Int32 SHA512_Update(ref SHA512_CTX c, Byte* data, IntPtr length)
            {
            return EnsureProcedure("SHA512_Update", ref FSHA512_Update)(ref c, data, length);
            }

        private delegate Int32 DSHA1_Init(ref SHA_CTX c);
        private delegate Int32 DSHA256_Init(ref SHA256_CTX c);
        private delegate Int32 DSHA512_Init(ref SHA512_CTX c);
        private unsafe delegate Int32 DSHA1_Update(ref SHA_CTX c, Byte* data, IntPtr length);
        private unsafe delegate Int32 DSHA256_Update(ref SHA256_CTX c, Byte* data, IntPtr length);
        private unsafe delegate Int32 DSHA512_Update(ref SHA512_CTX c, Byte* data, IntPtr length);
        private delegate Int32 DSHA1_Final(Byte[] digest, ref SHA_CTX c);
        private delegate Int32 DSHA256_Final(Byte[] digest, ref SHA256_CTX c);
        private delegate Int32 DSHA512_Final(Byte[] digest, ref SHA512_CTX c);

        private DSHA1_Init     FSHA1_Init;
        private DSHA256_Init   FSHA256_Init;
        private DSHA256_Init   FSHA224_Init;
        private DSHA512_Init   FSHA384_Init;
        private DSHA512_Init   FSHA512_Init;
        private DSHA1_Update   FSHA1_Update;
        private DSHA256_Update FSHA256_Update;
        private DSHA256_Update FSHA224_Update;
        private DSHA512_Update FSHA384_Update;
        private DSHA512_Update FSHA512_Update;
        private DSHA1_Final    FSHA1_Final;
        private DSHA256_Final  FSHA256_Final;
        private DSHA256_Final  FSHA224_Final;
        private DSHA512_Final  FSHA384_Final;
        private DSHA512_Final  FSHA512_Final;
        }
    }