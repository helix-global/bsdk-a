using System;

namespace BinaryStudio.Security.Cryptography
    {
    public interface ICryptKey : IDisposable
        {
        IntPtr Handle { get; }
        }
    }