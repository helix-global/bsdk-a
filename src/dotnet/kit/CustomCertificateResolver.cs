using System;
using System.Linq;
using BinaryStudio.Security.Cryptography.Certificates;

namespace Kit
    {
    public class CustomCertificateResolver : IX509CertificateResolver {
        public IX509Certificate Find(String serialnumber, String issuer) {
            foreach (var i in (new X509CertificateStorage(X509StoreName.My, X509StoreLocation.CurrentUser)).Certificates) {
                if (String.Equals(i.SerialNumber,serialnumber,StringComparison.OrdinalIgnoreCase)) {
                    return i;
                    }
                }
            throw new InvalidOperationException();
            }

        public IX509Certificate Find(String thumbprint) {
            foreach (var i in (new X509CertificateStorage(X509StoreName.My, X509StoreLocation.CurrentUser)).Certificates) {
                if (String.Equals(i.Thumbprint,thumbprint,StringComparison.OrdinalIgnoreCase)) {
                    return i;
                    }
                }
            throw new InvalidOperationException();
            }

        public IX509Certificate Find(Byte[] thumbprint)
            {
            return Find(String.Join(String.Empty, thumbprint.Select(i => i.ToString("X2"))));
            }
        }
    }