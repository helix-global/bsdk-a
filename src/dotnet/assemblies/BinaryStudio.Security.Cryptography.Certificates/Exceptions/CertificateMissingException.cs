using System;

namespace BinaryStudio.Security.Cryptography
    {
    public class CertificateMissingException : CertificateException
        {
        /// <summary>Initializes a new instance of the <see cref="CertificateMissingException"/> class with a specified message that describes the error.</summary>
        /// <param name="message">The message that describes the exception. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</param>
        public CertificateMissingException(String message)
            :base(message)
            {
            }
        }
    }