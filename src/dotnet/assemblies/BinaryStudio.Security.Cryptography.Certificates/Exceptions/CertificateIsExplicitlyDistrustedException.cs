﻿using System;
using BinaryStudio.Security.Cryptography.Certificates.Properties;
using Microsoft.Win32;

namespace BinaryStudio.Security.Cryptography
    {
    [CertificateChainErrorStatus(CertificateChainErrorStatus.CERT_TRUST_IS_EXPLICIT_DISTRUST)]
    public class CertificateIsExplicitlyDistrustedException : CertificateException
        {
        /// <summary>Initializes a new instance of the <see cref="CertificateIsExplicitlyDistrustedException"/> class with a specified message that describes the error.</summary>
        /// <param name="message">The message that describes the exception. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</param>
        public CertificateIsExplicitlyDistrustedException(String message)
            :base(message)
            {
            }

        /// <summary>Initializes a new instance of the <see cref="CertificateIsExplicitlyDistrustedException"/> class with a system-supplied message that describes the error.</summary>
        public CertificateIsExplicitlyDistrustedException()
            :base(Resources.ResourceManager.GetString(nameof(CertificateChainErrorStatus.CERT_TRUST_IS_EXPLICIT_DISTRUST)))
            {
            }
        }
    }