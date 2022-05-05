using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using BinaryStudio.PlatformComponents;
using Microsoft.Win32;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    [Serializable]
    internal abstract class CertificateStorage : IX509CertificateStorage, ISerializable
        {
        static CertificateStorage()
            {
            }

        protected CertificateStorage()
            {
            }

        protected CertificateStorage(SerializationInfo info, StreamingContext context)
            {
            if (info == null) { throw new ArgumentNullException(nameof(info)); }
            }

        public abstract X509StoreLocation Location { get; }
        public abstract String Name { get; }
        public abstract Boolean IsReadOnly { get; }
        public abstract IntPtr Handle { get; }

        #region M:Dispose(Boolean)
        protected virtual void Dispose(Boolean disposing)
            {
            }
        #endregion
        #region M:Dispose
        public void Dispose()
            {
            Dispose(true);
            GC.SuppressFinalize(this);
            }
        #endregion
        #region M:Finalize
        ~CertificateStorage()
            {
            Dispose(false);
            }
        #endregion
        #region M:Validate(Boolean)
        protected virtual void Validate(Boolean status) {
            if (!status) {
                throw PlatformContext.GetExceptionForHR(Marshal.GetLastWin32Error());
                }
            }
        #endregion

        protected abstract void EnsureCore();
        public abstract unsafe X509Certificate Find(CERT_INFO* value, out Exception e);
        public abstract IEnumerable<X509Certificate> Certificates { get; }

        /// <summary>
        /// Enums all certificate revocation lists in storage.
        /// </summary>
        public abstract IEnumerable<X509CertificateRevocationList> CertificateRevocationLists { get; }
        public abstract void Add(IX509Certificate o);
        public abstract void Add(IX509CertificateRevocationList o);
        public abstract void Remove(IX509Certificate o);
        public abstract void Remove(IX509CertificateRevocationList o);

        public abstract void Commit();

        IEnumerable<IX509Certificate> IX509CertificateStorage.Certificates { get {
            foreach (var i in Certificates) {
                yield return i;
                }
            }}

        /// <summary>
        /// Enums all certificate revocation lists in storage.
        /// </summary>
        IEnumerable<IX509CertificateRevocationList> IX509CertificateStorage.CertificateRevocationLists { get {
            foreach (var i in CertificateRevocationLists) {
                yield return i;
                }
            }}

        [DllImport("kernel32.dll", BestFitMapping = true, CharSet = CharSet.Unicode, SetLastError = true)] private static extern unsafe Boolean FormatMessage(UInt32 flags, IntPtr source,  Int32 dwMessageId, UInt32 dwLanguageId, void* lpBuffer, Int32 nSize, IntPtr[] arguments);

        /// <summary>Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with the data needed to serialize the target object.</summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext"/>) for this serialization.</param>
        /// <exception cref="T:System.Security.SecurityException">The caller does not have the required permission.</exception>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
            {
            if (info == null) { throw new ArgumentNullException(nameof(info)); }
            }
        }
    }
