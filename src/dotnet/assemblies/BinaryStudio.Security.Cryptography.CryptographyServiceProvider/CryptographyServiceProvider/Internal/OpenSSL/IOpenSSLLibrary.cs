using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    internal interface IOpenSSLLibrary
        {
        Int32 SHA1_Init(ref SHA_CTX c);
        Int32 SHA256_Init(ref SHA256_CTX c);
        Int32 SHA224_Init(ref SHA256_CTX c);
        Int32 SHA384_Init(ref SHA512_CTX c);
        Int32 SHA512_Init(ref SHA512_CTX c);
        Int32 SHA1_Final([MarshalAs(UnmanagedType.LPArray)]Byte[] digest, ref SHA_CTX c);
        Int32 SHA256_Final([MarshalAs(UnmanagedType.LPArray)]Byte[] digest, ref SHA256_CTX c);
        Int32 SHA224_Final([MarshalAs(UnmanagedType.LPArray)]Byte[] digest, ref SHA256_CTX c);
        Int32 SHA384_Final([MarshalAs(UnmanagedType.LPArray)]Byte[] digest, ref SHA512_CTX c);
        Int32 SHA512_Final([MarshalAs(UnmanagedType.LPArray)]Byte[] digest, ref SHA512_CTX c);
        unsafe Int32 SHA1_Update(ref SHA_CTX c, Byte* data, IntPtr length);
        unsafe Int32 SHA256_Update(ref SHA256_CTX c, Byte* data, IntPtr length);
        unsafe Int32 SHA224_Update(ref SHA256_CTX c, Byte* data, IntPtr length);
        unsafe Int32 SHA384_Update(ref SHA512_CTX c, Byte* data, IntPtr length);
        unsafe Int32 SHA512_Update(ref SHA512_CTX c, Byte* data, IntPtr length);
        }
    }