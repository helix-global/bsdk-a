using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using BinaryStudio.Diagnostics;
using BinaryStudio.Security.Cryptography.Certificates;
using BinaryStudio.Security.Cryptography.CryptographyServiceProvider;
using Options;
using Options.Descriptors;

namespace Operations
    {
    internal abstract class CreateOrEncryptMessageOperation : MessageOperation
        {
        public IList<InputCertificate> Certificates { get; }
        protected CreateOrEncryptMessageOperation(TextWriter output, TextWriter error, IList<OperationOption> args)
            : base(output, error, args)
            {
            Certificates = args.OfType<InputCertificateOption>().FirstOrDefault()?.Certificates;
            if (Certificates == null) {
                throw new OptionRequiredException(new InputCertificateOptionDescriptor());
                }
            }

        #region M:BuildCertificateStorage:IX509CertificateStorage
        private IX509CertificateStorage BuildCertificateStorage() {
            if (String.Equals(StoreName, "device", StringComparison.OrdinalIgnoreCase)) {
                using (var context = new SCryptographicContext(ProviderType, CryptographicContextFlags.CRYPT_SILENT| CryptographicContextFlags.CRYPT_VERIFYCONTEXT)) {
                    var storage = (IX509CertificateStorage)context.GetService(typeof(IX509CertificateStorage));
                    if (IsNullOrEmpty(Certificates)) { return storage; }
                    var r = new X509CertificateStorage();
                    foreach (var certificate in storage.Certificates) {
                        if (Certificates.Any(i => String.Equals(i.Thumbprint, certificate.Thumbprint, StringComparison.OrdinalIgnoreCase))) {
                            r.Add(certificate);
                            }
                        }
                    return r;
                    }
                }
            else
                {
                var storage = Directory.Exists(StoreName)
                    ? new X509CertificateStorage(new Uri(StoreName))
                    : new X509CertificateStorage(ToStoreName(StoreName).GetValueOrDefault(), StoreLocation);
                    if (IsNullOrEmpty(Certificates)) { return storage; }
                var r = new X509CertificateStorage();
                foreach (var certificate in storage.Certificates) {
                    Debug.Print($"Certificate:{certificate.Thumbprint}");
                    if (Certificates.Any(i => String.Equals(i.Thumbprint, certificate.Thumbprint, StringComparison.OrdinalIgnoreCase))) {
                        r.Add(certificate);
                        }
                    }
                return r;
                }
            }
        #endregion

        protected abstract void Execute(TextWriter output, CryptographicContext context, IX509CertificateStorage store);

        public sealed override void Execute(TextWriter output) {
            try
                {
                using (var context = new CryptographicContext(
                    Logger,ProviderType,
                    CryptographicContextFlags.CRYPT_SILENT | CryptographicContextFlags.CRYPT_VERIFYCONTEXT)) {
                    using (var store = BuildCertificateStorage()) {
                        Execute(output,context, store);
                        }
                    }
                }
            catch (Exception e)
                {
                e.Add("ProviderType", ProviderType);
                e.Add("StoreLocation", StoreLocation);
                e.Add("StoreName", StoreName);
                throw;
                }
            }
        }
    }