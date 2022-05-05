using System;
using BinaryStudio.Security.Cryptography.Certificates;

namespace BinaryStudio.Security.Cryptography
    {
    public interface ICryptKey : IDisposable
        {
        IntPtr Handle { get; }
        String Container { get; }
        IX509Certificate Certificate { get; }
        }
    }