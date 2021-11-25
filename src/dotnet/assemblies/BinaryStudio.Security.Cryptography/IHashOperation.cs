using System;
using BinaryStudio.PlatformComponents.Win32;

namespace BinaryStudio.Security.Cryptography
    {
    public interface IHashOperation : IDisposable
        {
        void HashCore(Byte[] array, Int32 startindex, Int32 size);
        Boolean VerifySignature(out Exception e, Byte[] signature, Byte[] digest, ICryptKey key);
        }
    }