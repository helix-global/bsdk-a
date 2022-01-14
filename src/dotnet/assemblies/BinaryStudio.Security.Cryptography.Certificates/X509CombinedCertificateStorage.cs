using System;
using System.Collections.Generic;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    public class X509CombinedCertificateStorage : IX509CertificateStorage
        {
        private IList<IX509CertificateStorage> storages = new List<IX509CertificateStorage>();
        private readonly Boolean flags;
        public X509CombinedCertificateStorage(Boolean flags, params IX509CertificateStorage[] storages)
            {
            this.flags = flags;
            foreach (var storage in storages) {
                if (storage != null) {
                    this.storages.Add(storage);
                    }
                }
            }

        #region M:Dispose(Boolean)
        protected virtual void Dispose(Boolean disposing) {
            if (storages != null) {
                if (flags) {
                    foreach (var storage in storages) { storage.Dispose(); }
                    }
                storages.Clear();
                storages = null;
                }
            }
        #endregion
        #region M:Dispose
        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
            {
            Dispose(true);
            GC.SuppressFinalize(this);
            }
        #endregion
        #region M:Finalize
        /// <summary>Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.</summary>
        ~X509CombinedCertificateStorage()
            {
            Dispose(false);
            }
        #endregion

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
            r.Clear();
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
            r.Clear();
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