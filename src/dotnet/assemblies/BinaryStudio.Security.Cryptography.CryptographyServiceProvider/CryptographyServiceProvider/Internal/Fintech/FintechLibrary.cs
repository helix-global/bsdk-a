using System;
using System.IO;
using System.Reflection;
using BinaryStudio.Diagnostics;
using BinaryStudio.Diagnostics.Logging;
using BinaryStudio.PlatformComponents;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    internal class FintechLibrary : IFintechLibrary
        {
        private readonly ILogger logger;
        private IFintechLibrary UnderlyingObject;
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

        public FintechLibrary(ILogger logger)
            {
            if (!File.Exists(LibraryFullPath)) { throw new FileNotFoundException(); }
            this.logger = logger;
            var version = Library.GetVersion(LibraryFullPath);
            Version = version;
            if ((version.CompareTo(new Version(1, 0, 123, 26)) >= 0) && (version.CompareTo(new Version(1,1)) < 0)) {
                UnderlyingObject = new FintechLibraryA(LibraryFullPath, version, logger);
                return;
                }
            //if ((version.CompareTo(new Version(1, 1)) > 0) && (version.CompareTo(new Version(1,2)) <= 0)) {
                UnderlyingObject = new FintechLibraryB(LibraryFullPath, version, logger);
                return;
                //}
            //throw new NotSupportedException();
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

        #region M:Dispose<T>([Ref]T)
        private static void Dispose<T>(ref T o)
            where T: class, IDisposable
            {
            if (o != null) {
                o.Dispose();
                o = null;
                }
            }
        #endregion
        #region M:Dispose(Boolean)
        /// <summary>
        /// Releases the unmanaged resources used by the instance and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
        private void Dispose(Boolean disposing) {
            Dispose(ref UnderlyingObject);
            }
        #endregion
        #region M:
        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
            {
            Dispose(true);
            GC.SuppressFinalize(this);
            }
        #endregion
        #region M:Finalize
        /// <summary>Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.</summary>
        ~FintechLibrary()
            {
            Dispose(false);
            }
        #endregion
        }
    }