using System;
using System.Collections.Generic;

namespace BinaryStudio.Security.Cryptography
    {
    public class CertificateException : SecurityException
        {
        /// <summary>Initializes a new instance of the <see cref="CertificateException"/> class with a specified message that describes the error.</summary>
        /// <param name="message">The message that describes the exception. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</param>
        protected CertificateException(String message)
            :base(message)
            {
            }

        /// <summary>Initializes a new instance of the <see cref="CertificateException"/> class with a specified error message and references to the inner exceptions that are the cause of this exception.</summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerExceptions">The exceptions that are the cause of the current exception.</param>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="innerExceptions"/> argument is null.</exception>
        /// <exception cref="T:System.ArgumentException">An element of <paramref name="innerExceptions"/> is null.</exception>
        protected CertificateException(String message, IEnumerable<Exception> innerExceptions)
            :base(message, innerExceptions)
            {
            }
        }
    }