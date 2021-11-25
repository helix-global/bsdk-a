using BinaryStudio.Diagnostics.Logging;
using BinaryStudio.Security.Cryptography.Certificates;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using Microsoft.Win32;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    internal class CryptographicContextStorage : CryptographicObject, IX509CertificateStorage
        {
        public override IntPtr Handle { get { return storage.Handle; }}

        #region M:IX509CertificateStorage.Add(IX509Certificate)
        void IX509CertificateStorage.Add(IX509Certificate o)
            {
            throw new NotSupportedException();
            }
        #endregion
        #region M:IX509CertificateStorage.Add(IX509CertificateRevocationList)
        public void Add(IX509CertificateRevocationList crl)
            {
            throw new NotSupportedException();
            }
        #endregion
        #region M:IX509CertificateStorage.Remove(IX509Certificate)
        void IX509CertificateStorage.Remove(IX509Certificate o)
            {
            throw new NotSupportedException();
            }
        #endregion
        #region M:IX509CertificateStorage.Remove(IX509CertificateRevocationList)
        void IX509CertificateStorage.Remove(IX509CertificateRevocationList o)
            {
            throw new NotSupportedException();
            }
        #endregion

        protected internal override ILogger Logger { get; }
        public CryptographicContextStorage(CryptographicContext context)
            {
            if (context == null) { throw new ArgumentNullException(nameof(context)); }
            this.context = context;
            Logger = context.Logger;
            }

        private readonly CryptographicContext context;
        private MemoryCertificateStorage storage;

        IEnumerable<IX509Certificate> IX509CertificateStorage.Certificates { get {
            foreach (var certificate in Certificates) {
                yield return certificate;
                }
            }}

        private unsafe void VerifyAccess(CryptographicContext context, Byte[] secdesc) {
            if (secdesc != null) {
                var i = new RawSecurityDescriptor(secdesc, 0);
                var c = WindowsIdentity.GetCurrent();
                fixed (Byte* bytes = secdesc) {
                    var r = (SECURITY_DESCRIPTOR*)bytes;
                    #if DEBUG
                    Debug.Print($"Control:{r->Control}");
                    #endif
                    }
                return;
                }
            }

        public IEnumerable<X509Certificate> Certificates { get {
            if (storage != null) {
                foreach (var i in Certificates) {
                    yield return i;
                    }
                yield break;
                }
            storage = new MemoryCertificateStorage();
            using (var provider = new CryptographicContext(context,
                CryptographicContextFlags.CRYPT_VERIFYCONTEXT|CryptographicContextFlags.CRYPT_SILENT|
                (context.UseMachineKeySet
                    ? CryptographicContextFlags.CRYPT_MACHINE_KEYSET
                    : CryptographicContextFlags.CRYPT_NONE))) {
                foreach (var key in provider.EnumUserKeys(false)) {
                    #if DEBUG
                    var message = new StringBuilder($"Container:[{key.Container}]:");
                    #endif
                    var r = key.GetParameter(KEY_PARAM.KP_CERTIFICATE);
                    if (r != null) {
                        if (key.Context.SecurityDescriptor != null) {
                            switch (context.Type) {
                                case CRYPT_PROVIDER_TYPE.PROV_GOST_2001_DH:
                                case CRYPT_PROVIDER_TYPE.PROV_GOST_2012_256:
                                case CRYPT_PROVIDER_TYPE.PROV_GOST_2012_512:
                                case CRYPT_PROVIDER_TYPE.PROV_GOST_94_DH:
                                    {
                                    }
                                    break;
                                }
                            }
                        var i = new X509Certificate(r, key.Container, key.KeySpec, provider.Type, provider.FullQualifiedContainerName, key.Context.SecurityDescriptor, provider.Name);
                        #if DEBUG
                        message.Append($"{{{i.Thumbprint}}}");
                        #endif
                        if (provider.IsSupported(i.SignatureAlgorithm)) {
                            storage.Add(i);
                            yield return i;
                            }
                        else
                            {
                            i.Dispose();
                            }
                        }
                    #if DEBUG
                    else
                        {
                        message.Append("{no certificate}");
                        }
                    #endif
                    #if DEBUG
                    if (Logger != null)
                        {
                        Logger.Log(LogLevel.Debug, message.ToString());
                        }
                    #endif
                    }
                }
            }}
        }
    }