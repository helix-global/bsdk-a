using System;
using BinaryStudio.PlatformComponents.Win32;
using BinaryStudio.Security.Cryptography.Certificates.Properties;
using Microsoft.Win32;

namespace BinaryStudio.Security.Cryptography
    {
    [CertificateChainErrorStatus(CertificateChainErrorStatus.CERT_TRUST_IS_OFFLINE_REVOCATION)]
    public class CertificateRevocationOfflineException : CertificateRevocationException
        {
        /// <summary>Initializes a new instance of the <see cref="CertificateRevocationOfflineException"/> class with a specified message that describes the error.</summary>
        /// <param name="message">The message that describes the exception. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</param>
        public CertificateRevocationOfflineException(String message)
            :base(message)
            {
            HResult = (Int32)HRESULT.CRYPT_E_REVOCATION_OFFLINE;
            }

        /// <summary>Initializes a new instance of the <see cref="CertificateRevocationOfflineException"/> class with a system-supplied message that describes the error.</summary>
        public CertificateRevocationOfflineException()
            :this(Resources.ResourceManager.GetString(nameof(CertificateChainErrorStatus.CERT_TRUST_IS_OFFLINE_REVOCATION)))
            {
            }
        }
    }