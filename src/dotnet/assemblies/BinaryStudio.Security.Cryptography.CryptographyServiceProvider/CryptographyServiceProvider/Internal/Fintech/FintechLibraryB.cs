using System;
using System.IO;
using BinaryStudio.Diagnostics.Logging;
using BinaryStudio.PlatformComponents;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    internal class FintechLibraryB : Library, IFintechLibrary
        {
        private readonly ILogger logger;
        public FintechLibraryB(String filepath, Version version, ILogger logger)
            : base(filepath)
            {
            this.logger = logger;
            Version = version;
            }

        public Version Version { get; }

        /// <summary>Verifies certificate using ICAO policy.</summary>
        /// <param name="handle">Certificate handle to verify.</param>
        void IFintechLibrary.VerifyMrtdCertificate(IntPtr handle)
            {
            throw new NotImplementedException();
            }

        /// <summary>Verifies CMS using ICAO policy.</summary>
        /// <param name="stream">Input stream containing MRTD CMS.</param>
        void IFintechLibrary.VerifyMrtdMessage(Stream stream)
            {
            throw new NotImplementedException();
            }
        }
    }