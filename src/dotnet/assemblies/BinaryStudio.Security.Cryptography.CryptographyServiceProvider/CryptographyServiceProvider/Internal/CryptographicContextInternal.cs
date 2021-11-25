using System;
using BinaryStudio.Diagnostics;
using BinaryStudio.Diagnostics.Logging;
using Microsoft.Win32;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    internal class CryptographicContextInternal : CryptographicObject
        {
        public override IntPtr Handle { get { return handle; }}
        protected internal override ILogger Logger { get; }

        protected CryptographicContextInternal(IntPtr handle, ILogger logger) {
            Logger = logger;
            this.handle = handle;
            }

        #region M:Dispose(Boolean)
        protected override void Dispose(Boolean disposing) {
            using (new TraceScope()) {
                if (disposing) {
                    if (handle != IntPtr.Zero) {
                        EntryPoint.CryptReleaseContext(handle, 0);
                        handle = IntPtr.Zero;
                        }
                    }
                base.Dispose(disposing);
                }
            }
        #endregion
        #region M:Create(CRYPT_PROVIDER_TYPE,IntPtr,ILogger):CryptographicContextInternal
        public static CryptographicContextInternal Create(CRYPT_PROVIDER_TYPE type, IntPtr handle, ILogger logger)
            {
            switch (type) {
                case CRYPT_PROVIDER_TYPE.PROV_GOST_2001_DH:
                case CRYPT_PROVIDER_TYPE.PROV_GOST_2012_256:
                case CRYPT_PROVIDER_TYPE.PROV_GOST_2012_512:
                case CRYPT_PROVIDER_TYPE.PROV_GOST_94_DH:
                    {
                    return new CryptoProCryptographicContext(handle, logger);
                    }
                }
            return new CryptographicContextInternal(handle, logger);
            }
        #endregion
        #region M:IsSecureCodeStored(CryptographicContext):Boolean
        public virtual Boolean IsSecureCodeStored(CryptographicContext context) {
            return false;
            }
        #endregion

        private IntPtr handle;
        }
    }