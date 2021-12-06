using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinaryStudio.Security.Cryptography.Certificates;
using BinaryStudio.Security.Cryptography.CryptographyServiceProvider;
using Microsoft.Win32;

namespace Kit
    {
    public class Utilities
        {
        #region M:ToStoreName(String):X509StoreName?
        private static X509StoreName? ToStoreName(String source) {
            if (source == null) { return X509StoreName.My; }
            switch (source.ToUpper())
                {
                case "ADDRESSBOOK"          : { return X509StoreName.AddressBook;          }
                case "AUTHROOT"             : { return X509StoreName.AuthRoot;             }
                case "CERTIFICATEAUTHORITY" : { return X509StoreName.CertificateAuthority; }
                case "DISALLOWED"           : { return X509StoreName.Disallowed;           }
                case "MY"                   : { return X509StoreName.My;                   }
                case "ROOT"                 : { return X509StoreName.Root;                 }
                case "TRUSTEDPEOPLE"        : { return X509StoreName.TrustedPeople;        }
                case "TRUSTEDPUBLISHER"     : { return X509StoreName.TrustedPublisher;     }
                case "TRUSTEDDEVICES"       : { return X509StoreName.TrustedDevices;       }
                case "NTAUTH"               : { return X509StoreName.NTAuth;               }
                }
            return null;
            }
        #endregion

        internal static IX509CertificateStorage BuildCertificateList(X509StoreLocation location, String storename, String certificates, CRYPT_PROVIDER_TYPE providertype) {
            if (storename == "device") {
                using (var context = new CryptographicContext(providertype, CryptographicContextFlags.CRYPT_SILENT| CryptographicContextFlags.CRYPT_VERIFYCONTEXT)) {
                    var storage = (IX509CertificateStorage)context.GetService(typeof(IX509CertificateStorage));
                    if (String.IsNullOrWhiteSpace(certificates)) { return storage; }
                    var values = certificates.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim().ToUpper().Replace(" ", "")).ToList();
                    var r = new X509CertificateStorage();
                    foreach (var certificate in storage.Certificates) {
                        if (values.Contains(certificate.Thumbprint.ToUpper())) {
                            r.Add(certificate);
                            }
                        }
                    return r;
                    }
                }
            else
                {
                var storage = Directory.Exists(storename)
                    ? new X509CertificateStorage(new Uri(storename))
                    : new X509CertificateStorage(ToStoreName(storename).GetValueOrDefault(), location);
                if (String.IsNullOrWhiteSpace(certificates)) { return storage; }
                var values = certificates.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim().ToUpper().Replace(" ", "")).ToList();
                var r = new X509CertificateStorage();
                foreach (var certificate in storage.Certificates) {
                    if (values.Contains(certificate.Thumbprint.ToUpper())) {
                        r.Add(certificate);
                        }
                    }
                return r;
                }
            }

        #region M:Hex(Byte[],TextWriter):String
        public static void Hex(Byte[] source, TextWriter writer) {
            if (source == null) {
                writer.WriteLine("(none)");
                return;
                }
            var i = 0;
            var sz = source.Length;
            var f = new List<Byte>();
            while (i < sz)
            {
                f.Clear();
                writer.Write("{0:X8}: ", i);
                var j = 0;
                for (; (j < 8) && (i < sz); ++j, ++i)
                {
                    writer.Write("{0:X2} ", source[i]);
                    f.Add(source[i]);
                }
                if (i < sz)
                {
                    writer.Write("| ");
                    j = 0;
                    for (; (j < 8) && (i < sz); ++j, ++i)
                    {
                        writer.Write("{0:X2} ", source[i]);
                        f.Add(source[i]);
                    }
                    if (i >= sz)
                    {
                        writer.Write(new String(' ', (8 - j) * 3));
                    }
                }
                else
                {
                    writer.Write(new String(' ', (16 - j) * 3 + 2));
                }
                foreach (var c in f)
                {
                    if (((c >= 'A') && (c <= 'Z')) ||
                        ((c >= 'a') && (c <= 'z')) ||
                        ((c >= '0') && (c <= '9')) ||
                        ((c >= 0x20) && (c <= 0x7E)))
                    {
                        writer.Write((char)c);
                    }
                    else
                    {
                        writer.Write('.');
                    }
                }
                writer.WriteLine();
            }
        }
        #endregion
        }
    }