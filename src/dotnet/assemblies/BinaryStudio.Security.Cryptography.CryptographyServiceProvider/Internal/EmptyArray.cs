using System;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider.Internal
    {
    internal static class EmptyArray<T>
        {
        #if NET40
        public static readonly T[] Value = new T[0];
        #else
        public static readonly T[] Value = Array.Empty<T>();
        #endif
        }
    }