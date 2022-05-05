using System;
using System.Collections.Generic;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    public interface IX509CertificateStorage : IDisposable
        {
        Boolean IsReadOnly { get; }
        IntPtr Handle { get; }
        IEnumerable<IX509Certificate> Certificates { get; }

        /// <summary>
        /// Enums all certificate revocation lists in storage.
        /// </summary>
        IEnumerable<IX509CertificateRevocationList> CertificateRevocationLists { get; }

        void Add(IX509Certificate o);
        void Add(IX509CertificateRevocationList o);
        void Remove(IX509Certificate o);
        void Remove(IX509CertificateRevocationList o);
        }
    }