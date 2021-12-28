using System;
using BinaryStudio.Security.Cryptography.Certificates.Properties;
using Microsoft.Win32;

namespace BinaryStudio.Security.Cryptography
    {
    [CertificateChainErrorStatus(CertificateChainErrorStatus.CERT_TRUST_HAS_EXCLUDED_NAME_CONSTRAINT)]
    public class CertificateExcludedNameConstraintException : CertificateConstraintException
        {
        /// <summary>Initializes a new instance of the <see cref="CertificateExcludedNameConstraintException"/> class with a specified message that describes the error.</summary>
        /// <param name="message">The message that describes the exception. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</param>
        public CertificateExcludedNameConstraintException(String message)
            : base(message)
            {
            }

        /// <summary>Initializes a new instance of the <see cref="CertificateExcludedNameConstraintException"/> class with a system-supplied message that describes the error.</summary>
        public CertificateExcludedNameConstraintException()
            :this(Resources.ResourceManager.GetString(nameof(CertificateChainErrorStatus.CERT_TRUST_HAS_EXCLUDED_NAME_CONSTRAINT)))
            {
            }
        }
    }