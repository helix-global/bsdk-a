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

        public Byte[] ReadAllBytes() {
            using (var inputstream = OpenRead()) {
                using (var outputstream = new MemoryStream()) {
                    inputstream.CopyTo(outputstream);
                    return outputstream.ToArray();
                    }
                }
            }

        public Stream OpenRead() {
            lock (this) {
                if (TemporalFileName == null) {
                    using (var sourcestream = ArchiveEntry.OpenEntryStream()) {
                        var assembly = Assembly.GetEntryAssembly();
                        var folder = Path.Combine(Path.GetTempPath(), $"{{{assembly.FullName}}}");
                        var filename = Path.GetTempFileName();
                        if (!Directory.Exists(folder)) { Directory.CreateDirectory(folder); }
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

        public void MoveTo(String target)
            {
            throw new NotImplementedException();
            }
        }
    }