using System;
using System.IO;
using BinaryStudio.PlatformUI.Properties;
using BinaryStudio.PlatformUI.Shell;
using BinaryStudio.Utilities;

namespace BinaryStudio.PlatformUI
    {
    public static class PathUtil
        {
        private static readonly Char[] DirectorySeparators = new Char[2]
        {
      Path.DirectorySeparatorChar,
      Path.AltDirectorySeparatorChar
        };
        private const String UncPrefix = "\\\\";

        public static String Normalize(String path)
            {
            if (String.IsNullOrEmpty(path))
                return path;
            if (path.IndexOfAny(Path.GetInvalidPathChars()) != -1)
                throw new ArgumentException(Resources.NormalizeError_InvalidPathChar, nameof(path));
            var index1 = 0;
            while (index1 < path.Length && Char.IsWhiteSpace(path[index1]))
                ++index1;
            if (index1 == path.Length)
                return String.Empty;
            var index2 = path.Length - 1;
            while (index2 >= index1 && Char.IsWhiteSpace(path[index2]))
                --index2;
            var num = index2 - index1 + 1;
            using (var reusableResourceHolder = ReusableStringBuilder.AcquireDefault(260))
                {
                var resource = reusableResourceHolder.Resource;
                var flag1 = false;
                var flag2 = false;
                for (var index3 = index1; index3 <= index2; ++index3)
                    {
                    var c = path[index3];
                    if (c == Path.AltDirectorySeparatorChar)
                        {
                        c = Path.DirectorySeparatorChar;
                        flag2 = true;
                        }
                    if (c == Path.DirectorySeparatorChar)
                        {
                        if (flag1 && index3 > index1 + 1)
                            {
                            flag2 = true;
                            continue;
                            }
                        }
                    else if (Char.IsUpper(c))
                        {
                        c = Char.ToLower(c);
                        flag2 = true;
                        }
                    flag1 = c == Path.DirectorySeparatorChar;
                    resource.Append(c);
                    }
                if (flag1 && resource.Length > 3)
                    {
                    resource.Remove(resource.Length - 1, 1);
                    flag2 = true;
                    }
                if (!flag2 && num == path.Length)
                    return path;
                return resource.ToString();
                }
            }

        public static Boolean IsNormalized(String path)
            {
            return path == Normalize(path);
            }

        public static String NormalizePath(this String path)
            {
            return Normalize(path);
            }

        public static Boolean IsNormalizedPath(this String path)
            {
            return IsNormalized(path);
            }

        public static Boolean IsDescendant(String parent, String child)
            {
            Validate.IsNotNullAndNotWhiteSpace(parent, "parent");
            Validate.IsNotNullAndNotWhiteSpace(child, "child");
            var length = parent.Length;
            while (IsDirectorySeparator(parent[length - 1]))
                --length;
            return child.Length >= length && String.Compare(parent, 0, child, 0, length, StringComparison.OrdinalIgnoreCase) == 0 && (child.Length <= length || child[length] == Path.DirectorySeparatorChar);
            }

        public static String GetCommonPathPrefix(String path1, String path2)
            {
            Validate.IsNotNull(path1, "path1");
            Validate.IsNotNull(path2, "path2");
            if (path1.Length == 0 || path2.Length == 0)
                return String.Empty;
            if (ArePathsEqual(path1, path2))
                return path1;
            var flag = path1.StartsWith("\\\\");
            if (flag != path2.StartsWith("\\\\"))
                return String.Empty;
            using (var reusableResourceHolder = ReusableStringBuilder.AcquireDefault(260))
                {
                var resource = reusableResourceHolder.Resource;
                var pathParser = new PathParser(path1);
                var other = new PathParser(path2);
                while (pathParser.MoveNext() && other.MoveNext() && (pathParser.CurrentLength == other.CurrentLength && pathParser.CompareCurrentSegment(other) == 0))
                    {
                    if (resource.Length == 0 & flag)
                        resource.Append("\\\\");
                    else if (resource.Length > 0)
                        resource.Append(Path.DirectorySeparatorChar);
                    resource.Append(path1, pathParser.CurrentStartIndex, pathParser.CurrentLength);
                    }
                if (resource.Length == 2 && resource[1] == Path.VolumeSeparatorChar)
                    resource.Append('\\');
                return resource.ToString();
                }
            }

        public static Boolean ArePathsEqual(String path1, String path2)
            {
            return String.Equals(path1, path2, StringComparison.OrdinalIgnoreCase);
            }

        public static Boolean IsRoot(String path)
            {
            if (String.IsNullOrWhiteSpace(path))
                return false;
            return ArePathsEqual(path, Path.GetPathRoot(path));
            }

        public static Boolean IsReparsePoint(String path)
            {
            Validate.IsNotNullAndNotEmpty(path, "path");
            if (Directory.Exists(path))
                {
                try
                    {
                    return (UInt32)(File.GetAttributes(path) & FileAttributes.ReparsePoint) > 0U;
                    }
                catch (FileNotFoundException ex)
                    {
                    }
                catch (DirectoryNotFoundException ex)
                    {
                    }
                }
            return false;
            }

        public static Boolean ContainsReparsePoint(String path, String pathRoot = null)
            {
            Validate.IsNotNullAndNotEmpty(path, "path");
            var path1 = path;
            var str = String.Empty;
            if (pathRoot != null)
                str = GetCommonPathPrefix(path, pathRoot);
            for (; path1 != null && path1.Length > str.Length; path1 = Path.GetDirectoryName(path1))
                {
                if (IsReparsePoint(path1))
                    return true;
                }
            return false;
            }

        public static Boolean IsDirectorySeparator(Char c)
            {
            if (c != Path.DirectorySeparatorChar)
                return c == Path.AltDirectorySeparatorChar;
            return true;
            }

        public static Boolean IsImplicitDirectory(String directory)
            {
            Validate.IsNotNull(directory, "directory");
            var fileName = Path.GetFileName(directory);
            switch (fileName.Length)
                {
                case 1:
                return fileName[0] == 46;
                case 2:
                if (fileName[1] != 46)
                    break;
                goto case 1;
                }
            return false;
            }

        public static String SafeGetExtension(String path)
            {
            if (path == null)
                return String.Empty;
            if (path.IndexOfAny(Path.GetInvalidPathChars()) == -1)
                return Path.GetExtension(path);
            var anyOf = new Char[3]
            {
        '.',
        Path.DirectorySeparatorChar,
        Path.AltDirectorySeparatorChar
            };
            var startIndex = path.LastIndexOfAny(anyOf);
            if (startIndex >= 0 && path[startIndex] == 46)
                return path.Substring(startIndex);
            return String.Empty;
            }

        private class PathParser
            {
            private readonly String _path;
            private Int32 _startIndex;
            private Int32 _length;

            public String Path
                {
                get
                    {
                    return _path;
                    }
                }

            public Int32 CurrentStartIndex
                {
                get
                    {
                    return _startIndex;
                    }
                }

            public Int32 CurrentLength
                {
                get
                    {
                    return _length;
                    }
                }

            public String Current
                {
                get
                    {
                    return _path.Substring(_startIndex, _length);
                    }
                }

            public PathParser(String path)
                {
                Validate.IsNotNull(path, "path");
                _path = path;
                }

            public Boolean MoveNext()
                {
                _startIndex = _startIndex + _length;
                while (_startIndex < _path.Length && IsDirectorySeparator(_path[_startIndex]))
                    _startIndex = _startIndex + 1;
                if (_startIndex >= _path.Length)
                    return false;
                var num = _path.IndexOfAny(DirectorySeparators, _startIndex);
                if (num == -1)
                    num = _path.Length;
                _length = num - _startIndex;
                return true;
                }

            public Int32 CompareCurrentSegment(PathParser other)
                {
                return String.Compare(_path, _startIndex, other._path, other._startIndex, _length, StringComparison.OrdinalIgnoreCase);
                }
            }
        }
    }