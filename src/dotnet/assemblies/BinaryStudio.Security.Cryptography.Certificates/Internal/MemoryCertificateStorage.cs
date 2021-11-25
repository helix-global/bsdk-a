using System;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    internal class MemoryCertificateStorage : CertificateStorageContext
        {
        #region M:EnsureCore
        protected override void EnsureCore() {
            if (store == IntPtr.Zero) {
                #region standard storage
                if ((Location == X509StoreLocation.CurrentUser) || (Location == X509StoreLocation.LocalMachine)) {
                    store = CertOpenStore(
                        CERT_STORE_PROV_MEMORY,
                        PKCS_7_ASN_ENCODING|X509_ASN_ENCODING,
                        IntPtr.Zero,
                        MapX509StoreFlags(Location,X509OpenFlags.MaxAllowed|X509OpenFlags.OpenExistingOnly),
                        IntPtr.Zero);
                    return;
                    }
                #endregion
                }
            }
        #endregion
        }
    }