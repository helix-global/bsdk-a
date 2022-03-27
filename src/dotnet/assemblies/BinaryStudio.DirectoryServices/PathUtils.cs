using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace BinaryStudio.DirectoryServices
    {
    public class PathUtils
        {
        private const Int32 MAX_PATH = 260;
        private static Boolean IsMatchPart(String pattern, String value) {
            if (String.IsNullOrWhiteSpace(pattern)) { return true; }
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
        }
    }