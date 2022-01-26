using System;
using System.IO;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    internal interface IFintechLibrary
        {
        Version Version { get; }

        /// <summary>Verifies certificate using ICAO policy.</summary>
        /// <param name="handle">Certificate handle to verify.</param>
        void VerifyMrtdCertificate(IntPtr handle);

        /// <summary>Verifies CMS using ICAO policy.</summary>
        /// <param name="stream">Input stream containing MRTD CMS.</param>
        void VerifyMrtdMessage(Stream stream);
        }
    }