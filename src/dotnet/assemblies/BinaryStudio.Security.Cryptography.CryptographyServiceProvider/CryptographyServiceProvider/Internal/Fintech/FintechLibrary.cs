using System;
using System.IO;
using System.Reflection;
using BinaryStudio.Diagnostics;
using BinaryStudio.PlatformComponents;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    internal class FintechLibrary : IFintechLibrary
        {
        private readonly IFintechLibrary UnderlyingObject;
        private static String fullpath;
        public static String LibraryFullPath { get {
            if (fullpath == null) {
                var filename = (IntPtr.Size == 4)
                    ? "csecapi32.dll"
                    : "csecapi64.dll";
                var executingassembly = Assembly.GetExecutingAssembly();
                var location = executingassembly.Location;
                    location = (String.IsNullOrEmpty(location))
                        ? AppDomain.CurrentDomain.BaseDirectory
                        : location;
                var module = ReplaceFileName(location, filename);
                if (!File.Exists(module)) {
                    location = (new Uri(executingassembly.CodeBase, UriKind.RelativeOrAbsolute)).AbsolutePath;
                    module = ReplaceFileName(Uri.UnescapeDataString(location), filename);
                    }
                fullpath = module;
                }
            return fullpath;
            }}

        private static String ReplaceFileName(String source, String filename) {
            if (String.IsNullOrEmpty(source)) { throw new ArgumentOutOfRangeException(nameof(source)); }
            if (String.IsNullOrEmpty(filename)) { throw new ArgumentOutOfRangeException(nameof(filename)); }
            source = source.Replace('/', '\\');
            for (var i = source.Length - 1; i >= 0; i--) {
                if (source[i] == '\\') {
                    return source.Substring(0, i + 1) + filename;
                    }
                }
            return filename;
            }

        public FintechLibrary()
            {
            if (!File.Exists(LibraryFullPath)) { throw new FileNotFoundException(); }
            var version = Library.GetVersion(LibraryFullPath);
            Version = version;
            if ((version.CompareTo(new Version(1, 0, 123, 26)) >= 0) && (version.CompareTo(new Version(1,1)) < 0)) {
                UnderlyingObject = new FintechLibraryA(LibraryFullPath, version);
                return;
                }
            if ((version.CompareTo(new Version(1, 1)) > 0) && (version.CompareTo(new Version(1,2)) < 0)) {
                UnderlyingObject = new FintechLibraryB(LibraryFullPath, version);
                return;
                }
            throw new NotSupportedException();
            }

        public Version Version { get; }

        /// <summary>Verifies certificate using ICAO policy.</summary>
        /// <param name="handle">Certificate handle to verify.</param>
        void IFintechLibrary.VerifyMrtdCertificate(IntPtr handle)
            {
            try
                {
                UnderlyingObject.VerifyMrtdCertificate(handle);
                }
            catch (Exception e)
                {
                e.Add("Version", Version.ToString());
                throw;
                }
            }

        /// <summary>Verifies CMS using ICAO policy.</summary>
        /// <param name="stream">Input stream containing MRTD CMS.</param>
        void IFintechLibrary.VerifyMrtdMessage(Stream stream)
            {
            try
                {
                UnderlyingObject.VerifyMrtdMessage(stream);
                }
            catch (Exception e)
                {
                e.Add("Version", Version.ToString());
                throw;
                }
            }
        }
    }