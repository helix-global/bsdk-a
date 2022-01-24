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
        private String TemporalFileName;

        public ArchiveEntryService(String filename, IArchiveEntry source) {
            ArchiveEntry = source;
            FileName = Path.Combine(filename, source.Key);
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