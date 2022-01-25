using System;
using System.IO;
using System.Reflection;
using SharpCompress.Archives;

namespace BinaryStudio.DirectoryServices.Internal
    {
    internal class ArchiveEntryService : IFileService
        {
        public IArchiveEntry ArchiveEntry { get; }
        public String FileName { get; }
        public String FullName { get; }
        private String TemporalFileName;

        public ArchiveEntryService(String filename, IArchiveEntry source) {
            ArchiveEntry = source;
            FullName = Path.Combine(filename, source.Key);
            FileName = source.Key;
            }

        Byte[] IFileService.ReadAllBytes() {
            using (var inputstream = OpenRead()) {
                using (var outputstream = new MemoryStream()) {
                    inputstream.CopyTo(outputstream);
                    return outputstream.ToArray();
                    }
                }
            }

        private void Reset()
            {
            if (TemporalFileName != null) {
                try
                    {
                    File.Delete(TemporalFileName);
                    }
                catch (Exception e)
                    {
                    }
                TemporalFileName = null;
                }
            }

        public Stream OpenRead() {
            lock (this) {
                if (TemporalFileName == null) {
                    using (var sourcestream = ArchiveEntry.OpenEntryStream()) {
                        var assembly = Assembly.GetEntryAssembly();
                        var folder = Path.Combine(Path.GetTempPath(), $"{{{assembly.FullName}}}");
                        if (!Directory.Exists(folder)) { Directory.CreateDirectory(folder); }
                        var filename = PathUtils.GetTempFileName(folder, "arc");
                        if (File.Exists(filename)) { File.Delete(filename); }
                        var block = new Byte[1024];
                        using (var output = File.OpenWrite(TemporalFileName = Path.Combine(folder, Path.GetFileName(filename)))) {
                            for (;;) {
                                var blockcount = block.Length;
                                var sourcecount = sourcestream.Read(block, 0, blockcount);
                                if (sourcecount == 0) { break; }
                                output.Write(block, 0, sourcecount);
                                }
                            }
                        }
                    }
                }
            return File.OpenRead(TemporalFileName);
            }

        void IFileService.MoveTo(String target) {
            MoveTo(target, false);
            }

        public void MoveTo(String target, Boolean overwrite) {
            if (target == null) { throw new ArgumentNullException(nameof(target)); }
            using (var sourcestream = OpenRead()) {
                if (File.Exists(target)) {
                    if (!overwrite) { throw new IOException(); }
                    File.Delete(target);
                    }
                var folder = Path.GetDirectoryName(target);
                if (!Directory.Exists(folder)) { Directory.CreateDirectory(folder); }
                using (var targetstream = File.OpenWrite(target)) {
                    sourcestream.CopyTo(targetstream);
                    }
                Reset();
                }
            }

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
        public void CopyTo(String target, Boolean overwrite) {
            if (target == null) { throw new ArgumentNullException(nameof(target)); }
            using (var sourcestream = OpenRead()) {
                if (File.Exists(target)) {
                    if (!overwrite) { throw new IOException(); }
                    File.Delete(target);
                    }
                var folder = Path.GetDirectoryName(target);
                if (!Directory.Exists(folder)) { Directory.CreateDirectory(folder); }
                using (var targetstream = File.OpenWrite(target)) {
                    sourcestream.CopyTo(targetstream);
                    }
                }
            }

        ~ArchiveEntryService() {
            Reset();
            }

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override String ToString()
            {
            return FileName;
            }
        }
    }