﻿using System;
using System.Collections.Generic;
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

        /// <summary>Initializes a new instance of the <see cref="CertificateRevocationOfflineException"/> class with a specified error message and references to the inner exceptions that are the cause of this exception.</summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerExceptions">The exceptions that are the cause of the current exception.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="innerExceptions"/> argument is null.</exception>
        /// <exception cref="T:System.ArgumentException">An element of <paramref name="innerExceptions"/> is null.</exception>
        public CertificateRevocationOfflineException(String message, IEnumerable<Exception> innerExceptions)
            :base(message, innerExceptions)
            {
            }
        }
    }