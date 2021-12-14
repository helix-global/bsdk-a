using System;
using System.Runtime.InteropServices;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    internal interface IOpenSSLLibrary
        {
        /// <summary>
        /// Initializes a <see cref="SHA_CTX"/> structure.
        /// </summary>
        Int32 SHA1_Init(ref SHA_CTX c);

        /// <summary>
        /// Initializes a <see cref="SHA256_CTX"/> structure.
        /// </summary>
        Int32 SHA256_Init(ref SHA256_CTX c);

        /// <summary>
        /// Initializes a <see cref="SHA256_CTX"/> structure.
        /// </summary>
        Int32 SHA224_Init(ref SHA256_CTX c);

        /// <summary>
        /// Initializes a <see cref="SHA512_CTX"/> structure.
        /// </summary>
        Int32 SHA384_Init(ref SHA512_CTX c);

        /// <summary>
        /// Initializes a <see cref="SHA512_CTX"/> structure.
        /// </summary>
        Int32 SHA512_Init(ref SHA512_CTX c);

        /// <summary>
        /// Places the message digest in <paramref name="digest"/>, which must have space for SHA_DIGEST_LENGTH == 20 bytes of output, and erases the <see cref="SHA_CTX"/>.
        /// </summary>
        /// <param name="digest">Message digest.</param>
        /// <param name="c">Context.</param>
        Int32 SHA1_Final([MarshalAs(UnmanagedType.LPArray)]Byte[] digest, ref SHA_CTX c);

        /// <summary>
        /// Places the message digest in <paramref name="digest"/>, which must have space for SHA_DIGEST_LENGTH == 32 bytes of output, and erases the <see cref="SHA256_CTX"/>.
        /// </summary>
        /// <param name="digest">Message digest.</param>
        /// <param name="c">Context.</param>
        Int32 SHA256_Final([MarshalAs(UnmanagedType.LPArray)]Byte[] digest, ref SHA256_CTX c);

        /// <summary>
        /// Places the message digest in <paramref name="digest"/>, which must have space for SHA_DIGEST_LENGTH == 28 bytes of output, and erases the <see cref="SHA256_CTX"/>.
        /// </summary>
        /// <param name="digest">Message digest.</param>
        /// <param name="c">Context.</param>
        Int32 SHA224_Final([MarshalAs(UnmanagedType.LPArray)]Byte[] digest, ref SHA256_CTX c);

        /// <summary>
        /// Places the message digest in <paramref name="digest"/>, which must have space for SHA_DIGEST_LENGTH == 64 bytes of output, and erases the <see cref="SHA512_CTX"/>.
        /// </summary>
        /// <param name="digest">Message digest.</param>
        /// <param name="c">Context.</param>
        Int32 SHA384_Final([MarshalAs(UnmanagedType.LPArray)]Byte[] digest, ref SHA512_CTX c);

        /// <summary>
        /// Places the message digest in <paramref name="digest"/>, which must have space for SHA_DIGEST_LENGTH == 48 bytes of output, and erases the <see cref="SHA512_CTX"/>.
        /// </summary>
        /// <param name="digest">Message digest.</param>
        /// <param name="c">Context.</param>
        Int32 SHA512_Final([MarshalAs(UnmanagedType.LPArray)]Byte[] digest, ref SHA512_CTX c);
        unsafe Int32 SHA1_Update(ref SHA_CTX c, Byte* data, IntPtr length);
        unsafe Int32 SHA256_Update(ref SHA256_CTX c, Byte* data, IntPtr length);
        unsafe Int32 SHA224_Update(ref SHA256_CTX c, Byte* data, IntPtr length);
        unsafe Int32 SHA384_Update(ref SHA512_CTX c, Byte* data, IntPtr length);
        unsafe Int32 SHA512_Update(ref SHA512_CTX c, Byte* data, IntPtr length);
        }
    }