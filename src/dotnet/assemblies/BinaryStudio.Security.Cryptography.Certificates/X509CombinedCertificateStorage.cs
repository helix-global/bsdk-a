using System;
using System.Collections.Generic;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    public class X509CombinedCertificateStorage : IX509CertificateStorage
        {
        private IList<IX509CertificateStorage> storages = new List<IX509CertificateStorage>();
        public X509CombinedCertificateStorage(params IX509CertificateStorage[] storages)
            {
            foreach (var storage in storages) {
                if (storage != null) {
                    this.storages.Add(storage);
                    }
                }
            }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose() {
            if (storages != null) {
                storages.Clear();
                storages = null;
                }
            }

        public Boolean IsReadOnly { get { return true; }}
        public IntPtr Handle { get { return IntPtr.Zero; }}

        public IEnumerable<IX509Certificate> Certificates { get {
            var r = new HashSet<String>();
            foreach (var storage in storages) {
                foreach (var i in storage.Certificates) {
                    if (r.Add(i.Thumbprint)) {
                        yield return i;
                        }
                    }
                }
            }}

        /// <summary>
        /// Enums all certificate revocation lists in storage.
        /// </summary>
        public IEnumerable<IX509CertificateRevocationList> CertificateRevocationLists { get {
            var r = new HashSet<String>();
            foreach (var storage in storages) {
                foreach (var i in storage.CertificateRevocationLists) {
                    if (r.Add(i.Thumbprint)) {
                        yield return i;
                        }
                    }
                }
            }}

        void IX509CertificateStorage.Add(IX509Certificate o)
            {
            throw new NotSupportedException();
            }

        void IX509CertificateStorage.Add(IX509CertificateRevocationList o)
            {
            throw new NotSupportedException();
            }

        void IX509CertificateStorage.Remove(IX509Certificate o)
            {
            throw new NotSupportedException();
            }

        void IX509CertificateStorage.Remove(IX509CertificateRevocationList o)
            {
            throw new NotSupportedException();
            }
        }
    }