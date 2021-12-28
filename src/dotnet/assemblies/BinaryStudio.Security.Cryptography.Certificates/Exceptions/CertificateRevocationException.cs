using System;
using BinaryStudio.PlatformComponents.Win32;

namespace BinaryStudio.Security.Cryptography
    {
    public class CertificateRevocationException : CertificateException
        {
        /// <summary>Initializes a new instance of the <see cref="CertificateRevocationException"/> class with a specified message that describes the error.</summary>
        /// <param name="message">The message that describes the exception. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</param>
        public CertificateRevocationException(String message)
            :base(message)
            {
            HResult = (Int32)HRESULT.CRYPT_E_NO_REVOCATION_CHECK;
            }
        }
    }