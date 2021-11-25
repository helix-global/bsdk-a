using System;
using System.Collections.Generic;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    public interface IX509CertificateStorage : IDisposable
        {
        IEnumerable<IX509Certificate> Certificates { get; }
        IntPtr Handle { get; }

        void Add(IX509Certificate o);
        void Add(IX509CertificateRevocationList o);
        void Remove(IX509Certificate o);
        void Remove(IX509CertificateRevocationList o);
        }
    }