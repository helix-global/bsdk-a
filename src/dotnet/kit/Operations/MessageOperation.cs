using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BinaryStudio.Security.Cryptography.Certificates;
using Microsoft.Win32;
using Options;

namespace Operations
    {
    internal abstract class MessageOperation : Operation
        {
        public CRYPT_PROVIDER_TYPE ProviderType { get; }
        public X509StoreLocation StoreLocation { get; }
        public IList<String> InputFileName { get; }
        public IList<String> OutputFileName { get; }
        public String StoreName { get; }

        protected MessageOperation(TextWriter output, TextWriter error, IList<OperationOption> args)
            : base(output, error, args)
            {
            ProviderType   = (CRYPT_PROVIDER_TYPE)args.OfType<ProviderTypeOption>().First().Type;
            StoreLocation  = args.OfType<StoreLocationOption>().First().Value;
            StoreName      = args.OfType<StoreNameOption>().First().Value;
            InputFileName  = args.OfType<InputFileOrFolderOption>().FirstOrDefault()?.Values;
            OutputFileName = args.OfType<OutputFileOrFolderOption>().FirstOrDefault()?.Values;
            }

        protected class CustomCertificateResolver : IX509CertificateResolver
            {
            public X509StoreLocation StoreLocation { get; }
            public String StoreName { get; }
            private TextWriter Writer { get; }

            public CustomCertificateResolver(String storename, X509StoreLocation location, TextWriter writer)
                {
                StoreLocation = location;
                StoreName = storename;
                Writer = writer;
                }

            public IX509Certificate Find(String serialnumber, String issuer) {
                foreach (var i in (new X509CertificateStorage(StoreName, StoreLocation)).Certificates) {
                    if (String.Equals(i.SerialNumber,serialnumber,StringComparison.OrdinalIgnoreCase)) {
                        return i;
                        }
                    }
                if (Writer != null) {
                    Writer.WriteLine($"Попытка найти сертификат не удалась: {serialnumber},{issuer}");
                    }
                return null;
                }

            public IX509Certificate Find(String thumbprint) {
                foreach (var i in (new X509CertificateStorage(StoreName, StoreLocation)).Certificates) {
                    if (String.Equals(i.Thumbprint,thumbprint,StringComparison.OrdinalIgnoreCase)) {
                        return i;
                        }
                    }
                if (Writer != null) {
                    Writer.WriteLine($"Попытка найти сертификат не удалась: {thumbprint}");
                    }
                return null;
                }

            public IX509Certificate Find(Byte[] thumbprint)
                {
                return Find(String.Join(String.Empty, thumbprint.Select(i => i.ToString("X2"))));
                }
            }

        #region M:ToStoreName(String):X509StoreName?
        protected static X509StoreName? ToStoreName(String source) {
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
        }
    }