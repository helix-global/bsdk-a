using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BinaryStudio.DirectoryServices;
using BinaryStudio.IO;

internal class HexGroupService : IDirectoryService
    {
    private static readonly Byte[] HeaderPattern = {
        0x3d,0x3d,0x3d,0x3d,0x3d,0x3d,0x3d,0x3d,0x3d,0x3d,0x3d,0x3d,0x3d,0x3d,0x3d,0x3d,
        0x3d,0x3d,0x3d,0x3d,0x3d,0x3d,0x3d,0x3d,0x3d,0x3d,0x3d,0x3d,0x3d,0x3d,0x3d,0x3d,
        0x3d,0x3d,0x3d,0x3d,0x3d,0x3d,0x3d,0x3d,0x3d,0x3d,0x3d,0x3d,0x3d,0x3d,0x3d,0x3d,
        0x3d,0x3d,0x3d,0x3d,0x3d,0x3d,0x3d,0x3d,0x3d,0x3d,0x3d,0x3d,0x3d,0x3d,0x3d,0x3d,
        0x0d,0x0a
        };
    public IFileService Source { get; }
    public HexGroupService(IFileService source)
        {
        Source = source;
        }

    private static Int64 FindPattern(Stream sourcestream, Byte[] pattern) {
        var count = pattern.Length;
        var buffer = new Byte[count];
        using (sourcestream.StorePosition()) {
            for (;;) {
                var flags = true;
                var sz = sourcestream.Read(buffer, 0, count);
                if (sz != count) { return -1; }
                for (var i = 0; i < count; i++) {
                    if (buffer[i] != pattern[i]) {
                        flags = false;
                        break;
                        }
                    }
                if (flags)
                    {
                    return sourcestream.Position - count;
                    }
                sourcestream.Seek(-count + 1, SeekOrigin.Current);
                }
            }
        }

    public IEnumerable<IFileService> GetFiles(String searchpattern, DirectoryServiceSearchOptions searchoption) {
        var fileindex = 0;
        using (var sourcestream = Source.OpenRead()) {
            for (;;) {
                var fileid = new StringBuilder();
                sourcestream.Seek(64, SeekOrigin.Current);
                sourcestream.Seek( 2, SeekOrigin.Current);
                while (true) {
                    var c = PeekByte(sourcestream);
                    if (c == 13) { break; }
                    if (c == -1)
                        {
                        yield break;
                        }
                    fileid.Append((char)sourcestream.ReadByte());
                    }
                sourcestream.Seek( 2, SeekOrigin.Current);
                sourcestream.Seek(64, SeekOrigin.Current);
                sourcestream.Seek( 2, SeekOrigin.Current);
                sourcestream.Seek(64, SeekOrigin.Current);
                var offset = FindPattern(sourcestream, HeaderPattern);
                if (offset == -1) {
                    using (var body = new MemoryStream()) {
                        sourcestream.CopyTo(body, 1024);
                        yield return new HexFile(body.ToArray(),
                            fileid.ToString(), fileindex,
                            Source.FileName);
                        }
                    }
                else
                    {
                    using (var body = new MemoryStream()) {
                        sourcestream.CopyTo(body, 1024, offset - sourcestream.Position);
                        yield return new HexFile(body.ToArray(),
                            fileid.ToString(), fileindex,
                            Source.FileName);
                        }
                    sourcestream.Position = offset;
                    }
                fileindex++;
                }
            }
        }

    private static Int32 PeekByte(Stream source) {
        try
            {
            return source.ReadByte();
            }
        finally
            {
            source.Seek(-1, SeekOrigin.Current);
            }
        }

    /// <summary>Returns a string that represents the current object.</summary>
    /// <returns>A string that represents the current object.</returns>
    public override String ToString()
        {
        return Source.ToString();
        }
    }
