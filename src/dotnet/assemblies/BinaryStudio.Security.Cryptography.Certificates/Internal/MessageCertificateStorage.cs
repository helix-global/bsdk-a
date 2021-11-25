using System;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    internal class MessageCertificateStorage : CertificateStorageContext
        {
        public override X509StoreLocation Location { get { return X509StoreLocation.CurrentUser; }}
        public override String Name { get { return "Message"; }}
        private readonly IntPtr message;

        public MessageCertificateStorage(IntPtr message)
            {
            if (message == IntPtr.Zero) { throw new ArgumentOutOfRangeException(nameof(message)); }
            this.message = message;
            }

        #region M:EnsureCore
        protected override void EnsureCore() {
            if (store == IntPtr.Zero) {
                #region standard storage
                if ((Location == X509StoreLocation.CurrentUser) || (Location == X509StoreLocation.LocalMachine)) {
                    store = CertOpenStore(
                        CERT_STORE_PROV_MSG,
                        PKCS_7_ASN_ENCODING|X509_ASN_ENCODING,
                        IntPtr.Zero,
                        MapX509StoreFlags(Location,X509OpenFlags.MaxAllowed|X509OpenFlags.OpenExistingOnly),
                        message);
                    }
                #endregion
                }
            }
        #endregion
        }
    }