using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    internal interface IOpenSSLLibrary
        {
        Int32 SHA256_Init(ref SHA256_CTX c);
        Int32 SHA256_Final([MarshalAs(UnmanagedType.LPArray)]Byte[] digest, ref SHA256_CTX c);
        Int32 SHA224_Init(ref SHA256_CTX c);
        Int32 SHA224_Final([MarshalAs(UnmanagedType.LPArray)]Byte[] digest, ref SHA256_CTX c);
        unsafe Int32 SHA256_Update(ref SHA256_CTX c, Byte* data, IntPtr length);
        unsafe Int32 SHA224_Update(ref SHA256_CTX c, Byte* data, IntPtr length);
        }
    }