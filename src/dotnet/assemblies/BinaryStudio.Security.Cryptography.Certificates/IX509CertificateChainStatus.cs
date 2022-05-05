using Microsoft.Win32;

namespace BinaryStudio.Security.Cryptography.Certificates
    {
    public interface IX509CertificateChainStatus
        {
        /// <summary>
        /// Error status codes.
        /// </summary>
        CertificateChainErrorStatus ErrorStatus { get; }

        /// <summary>
        /// Information status codes.
        /// </summary>
        CertificateChainInfoStatus  InfoStatus  { get; }
        }
    }