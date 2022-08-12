using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using BinaryStudio.PlatformComponents.Win32;

namespace BinaryStudio.DirectoryServices
    {
    public class PathUtils
        {
        private const Int32 MAX_PATH = 260;
        private static Boolean IsMatchPart(String pattern, String value) {
            #if NET35
            if (String.IsNullOrEmpty(pattern)) { return true; }
            #else
            if (String.IsNullOrWhiteSpace(pattern)) { return true; }
            #endif
            if ((pattern == "*") || (pattern == "*.*")) { return true; }
            if (String.Equals(pattern, value, StringComparison.OrdinalIgnoreCase)) { return true; }
            if (pattern.EndsWith  ("*") &&
                pattern.StartsWith("*"))
                {
                return value.Contains(pattern.Trim('*'));
                }
            if (pattern.EndsWith  ("*")) { return value.StartsWith(pattern.TrimEnd('*')); }
            if (pattern.StartsWith("*")) { return value.EndsWith(pattern.TrimStart('*')); }
            return false;
            }

        public static Boolean IsMatch(String pattern, String filename)
            {
            if ((pattern == "*.*") || (pattern == "*")) { return true; }
            var pD = Path.GetDirectoryName(pattern);
            var pF = Path.GetFileNameWithoutExtension(pattern);
            var pE = Path.GetExtension(pattern);
            var iD = Path.GetDirectoryName(filename);
            var iF = Path.GetFileNameWithoutExtension(filename);
            var iE = Path.GetExtension(filename);
            return IsMatchPart(pD,iD)
                && IsMatchPart(pE,iE)
                && IsMatchPart(pF,iF);
            }

        /// <summary>
        /// Creates a uniquely named, zero-byte temporary file on disk and returns the full path of that file.
        /// </summary>
        /// <param name="folder">The directory path for the file name.</param>
        /// <returns>The full path of the temporary file.</returns>
        public static String GetTempFileName(String folder) {
            return GetTempFileName(folder, "tmp");
            }

        /// <summary>
        /// Creates a uniquely named, zero-byte temporary file on disk and returns the full path of that file.
        /// </summary>
        /// <param name="folder">The directory path for the file name.</param>
        /// <param name="prefix">The name prefix.</param>
        /// <returns>The full path of the temporary file.</returns>
        public static String GetTempFileName(String folder, String prefix) {
            var r = new StringBuilder(MAX_PATH);
            GetTempFileName(folder, prefix, 0U, r);
            return r.ToString();
            }


        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true, BestFitMapping = false)] internal static extern UInt32 GetTempFileName(String tmppath,String prefix,UInt32 uniqueIdOrZero,[Out] StringBuilder tmpFileName);

        public static Boolean IsSame(String x, String y)
            {
            if (String.Equals(x, y)) { return true; }
            if ((x == null) || (y == null)) { return false; }
            if (x.StartsWith(@"\\?\")) { x = x.Substring(4); }
            if (y.StartsWith(@"\\?\")) { y = y.Substring(4); }
            if (String.Equals(x, y)) { return true; }
            return false;
            }

        public static HRESULT SetLastAccessTime(String path, DateTime value)
            {
            try
                {
                File.SetLastAccessTime(path,value);
                return HRESULT.S_OK;
                }
            catch (Exception e)
                {
                return (HRESULT)Marshal.GetHRForException(e);
                }
            }

        public static HRESULT SetLastWriteTime(String path, DateTime value)
            {
            try
                {
                File.SetLastWriteTime(path,value);
                return HRESULT.S_OK;
                }
            catch (Exception e)
                {
                return (HRESULT)Marshal.GetHRForException(e);
                }
            }

        public static HRESULT SetCreationTime(String path, DateTime value)
            {
            try
                {
                File.SetCreationTime(path,value);
                return HRESULT.S_OK;
                }
            catch (Exception e)
                {
                return (HRESULT)Marshal.GetHRForException(e);
                }
            }
        }
    }