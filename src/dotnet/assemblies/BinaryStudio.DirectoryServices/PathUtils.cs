using System;
using System.IO;

namespace BinaryStudio.DirectoryServices
    {
    public class PathUtils
        {
        private static Boolean IsMatchPart(String pattern, String value) {
            if (String.IsNullOrWhiteSpace(pattern)) { return true; }
            if ((pattern == "*") || (pattern == "*.*")) { return true; }
            return String.Equals(pattern, value, StringComparison.OrdinalIgnoreCase);
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
                && IsMatchPart(pF,iF)
                && IsMatchPart(pE,iE);
            }
        }
    }