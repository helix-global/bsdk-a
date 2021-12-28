using System;
using BinaryStudio.PlatformComponents.Win32;
using BinaryStudio.Security.Cryptography.Certificates.Properties;
using Microsoft.Win32;

namespace BinaryStudio.Security.Cryptography
    {
    [CertificateChainErrorStatus(CertificateChainErrorStatus.CERT_TRUST_IS_REVOKED)]
    public class CertificateIsRevokedException : CertificateRevocationException
        {
        /// <summary>Initializes a new instance of the <see cref="CertificateIsRevokedException"/> class with a specified message that describes the error.</summary>
        /// <param name="message">The message that describes the exception. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</param>
        public CertificateIsRevokedException(String message)
            :base(message)
            {
            HResult = (Int32)HRESULT.CRYPT_E_REVOKED;
            }

        /// <summary>Initializes a new instance of the <see cref="CertificateIsRevokedException"/> class with a system-supplied message that describes the error.</summary>
        public CertificateIsRevokedException()
            :this(Resources.ResourceManager.GetString(nameof(CertificateChainErrorStatus.CERT_TRUST_IS_REVOKED)))
            {
            }
        }
    }