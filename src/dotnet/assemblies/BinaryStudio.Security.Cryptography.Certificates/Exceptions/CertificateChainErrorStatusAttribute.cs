using System;
using Microsoft.Win32;

namespace BinaryStudio.Security.Cryptography
    {
    internal class CertificateChainErrorStatusAttribute : Attribute
        {
        public CertificateChainErrorStatus Status { get; }
        public CertificateChainErrorStatusAttribute(CertificateChainErrorStatus status)
            {
            Status = status;
            }
        }
    }