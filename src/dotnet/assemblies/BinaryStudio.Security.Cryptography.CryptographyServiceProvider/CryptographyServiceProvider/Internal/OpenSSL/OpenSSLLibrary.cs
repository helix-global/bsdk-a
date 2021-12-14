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
        }
    }