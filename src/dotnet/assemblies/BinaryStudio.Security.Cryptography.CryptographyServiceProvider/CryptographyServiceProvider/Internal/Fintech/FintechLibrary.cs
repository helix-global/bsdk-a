using System;
using System.IO;
using System.Reflection;
using BinaryStudio.PlatformComponents;

namespace BinaryStudio.Security.Cryptography.CryptographyServiceProvider
    {
    internal class FintechLibrary
        {
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
            }
        }
    }