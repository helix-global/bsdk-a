using System;
using System.Runtime.InteropServices;
using BinaryStudio.Diagnostics;
using BinaryStudio.Diagnostics.Logging;
using Microsoft.Win32;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    internal class CryptographicSecureCodeStorageContext : CryptographicObject
        {
        public override IntPtr Handle { get { return handle; }}
        protected internal override ILogger Logger { get; }

        protected CryptographicSecureCodeStorageContext(IntPtr handle, ILogger logger) {
            Logger = logger;
            this.handle = handle;
            }

        #region M:Dispose(Boolean)
        protected override void Dispose(Boolean disposing) {
            using (new TraceScope()) {
                if (disposing) {
                    if (handle != IntPtr.Zero) {
                        CryptReleaseContext(handle, 0);
                        handle = IntPtr.Zero;
                        }
                    }
                base.Dispose(disposing);
                }
            }
        #endregion
        #region M:Create(CRYPT_PROVIDER_TYPE,IntPtr,ILogger):CryptographicContextInternal
        public static CryptographicSecureCodeStorageContext Create(CRYPT_PROVIDER_TYPE type, IntPtr handle, ILogger logger)
            {
            switch (type) {
                case CRYPT_PROVIDER_TYPE.PROV_GOST_2001_DH:
                case CRYPT_PROVIDER_TYPE.PROV_GOST_2012_256:
                case CRYPT_PROVIDER_TYPE.PROV_GOST_2012_512:
                case CRYPT_PROVIDER_TYPE.PROV_GOST_94_DH:
                    {
                    return new CryptoProCryptographicSecureCodeStorageContext(handle, logger);
                    }
                }
            return new CryptographicSecureCodeStorageContext(handle, logger);
            }
        #endregion
        #region M:IsSecureCodeStored(CryptographicContext):Boolean
        public virtual Boolean IsSecureCodeStored(CryptographicContext context) {
            return false;
            }
        #endregion

        private IntPtr handle;

        [DllImport("advapi32.dll", BestFitMapping = false, CharSet = CharSet.Auto, SetLastError = true)] private static extern Boolean CryptReleaseContext(IntPtr handle, Int32 flags);
        }
    }