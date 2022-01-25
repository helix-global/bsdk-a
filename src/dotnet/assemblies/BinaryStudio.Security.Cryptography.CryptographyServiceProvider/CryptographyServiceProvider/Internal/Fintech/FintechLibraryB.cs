using System;
using BinaryStudio.PlatformComponents;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    internal class FintechLibraryB : Library, IFintechLibrary
        {
        public FintechLibraryB(String filepath)
            : base(filepath)
            {
            }

        /// <summary>Verifies certificate using ICAO policy.</summary>
        /// <param name="handle">Certificate handle to verify.</param>
        public void VerifyMrtdCertificate(IntPtr handle)
            {
            throw new NotImplementedException();
            }
        }
    }