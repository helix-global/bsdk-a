using System;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    internal interface IFintechLibrary
        {
        /// <summary>Verifies certificate using ICAO policy.</summary>
        /// <param name="handle">Certificate handle to verify.</param>
        void VerifyMrtdCertificate(IntPtr handle);
        }
    }