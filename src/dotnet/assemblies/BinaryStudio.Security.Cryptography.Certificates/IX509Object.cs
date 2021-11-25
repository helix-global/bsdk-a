using System;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    public interface IX509Object
        {
        X509ObjectType ObjectType { get; }
        IntPtr Handle { get; }
        }
    }
