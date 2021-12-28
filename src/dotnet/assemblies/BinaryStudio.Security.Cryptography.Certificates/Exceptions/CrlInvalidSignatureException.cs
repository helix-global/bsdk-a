using System;

namespace BinaryStudio.Security.Cryptography
    {
    public class CrlInvalidSignatureException : CrlException
        {
        /// <summary>Initializes a new instance of the <see cref="CrlInvalidSignatureException"/> class with a specified message that describes the error.</summary>
        /// <param name="message">The message that describes the exception. The caller of this constructor is required to ensure that this string has been localized for the current system culture.</param>
        public CrlInvalidSignatureException(String message)
            : base(message)
            {
            }
        }
    }