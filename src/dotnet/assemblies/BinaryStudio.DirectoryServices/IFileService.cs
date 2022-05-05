using System;
using System.IO;

namespace BinaryStudio.DirectoryServices
    {
    public interface IFileService
        {
        String FileName { get; }
        String FullName { get; }
        Byte[] ReadAllBytes();
        Stream OpenRead();
        void MoveTo(String target);


        /// <summary>Move an existing file to a new file. Overwriting a file of the same name is allowed.</summary>
        /// <param name="target">The name of the destination file. This cannot be a directory.</param>
        /// <param name="overwrite"><see langword="true"/> if the destination file can be overwritten; otherwise, <see langword="false"/>.</param>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. -or-  <paramref name="target"/> is read-only.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="target"/> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by <see cref="F:System.IO.Path.InvalidPathChars"/>.  -or-  <paramref name="target"/> specifies a directory.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.IO.PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length.</exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The path specified in <paramref name="target"/> is invalid (for example, it is on an unmapped drive).</exception>
        /// <exception cref="T:System.IO.IOException"><paramref name="target"/> exists and <paramref name="overwrite"/> is <see langword="false"/>. -or- An I/O error has occurred.</exception>
        /// <exception cref="T:System.NotSupportedException"><paramref name="target"/> is in an invalid format.</exception>
        void MoveTo(String target, Boolean overwrite);

        /// <summary>Copies an existing file to a new file. Overwriting a file of the same name is allowed.</summary>
        /// <param name="target">The name of the destination file. This cannot be a directory.</param>
        /// <param name="overwrite"><see langword="true"/> if the destination file can be overwritten; otherwise, <see langword="false"/>.</param>
        /// <exception cref="T:System.UnauthorizedAccessException">The caller does not have the required permission. -or-  <paramref name="target"/> is read-only.</exception>
        /// <exception cref="T:System.ArgumentException"><paramref name="target"/> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by <see cref="F:System.IO.Path.InvalidPathChars"/>.  -or-  <paramref name="target"/> specifies a directory.</exception>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
        /// <exception cref="T:System.IO.PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length.</exception>
        /// <exception cref="T:System.IO.DirectoryNotFoundException">The path specified in <paramref name="target"/> is invalid (for example, it is on an unmapped drive).</exception>
        /// <exception cref="T:System.IO.IOException"><paramref name="target"/> exists and <paramref name="overwrite"/> is <see langword="false"/>. -or- An I/O error has occurred.</exception>
        /// <exception cref="T:System.NotSupportedException"><paramref name="target"/> is in an invalid format.</exception>
        void CopyTo(String target, Boolean overwrite);
        }
    }