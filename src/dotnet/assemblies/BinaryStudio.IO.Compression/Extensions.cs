using System;
using System.IO;

namespace BinaryStudio.IO.Compression
    {
    public static class Extensions
        {
        internal static Int64 ReadVInt64(this BinaryReader reader) {
            if (reader == null) { throw new ArgumentNullException(nameof(reader)); }
            var sf = 0;
            var r  = 0L;
            for (;(sf < 64);sf += 7) {
                var i = reader.ReadByte();
                r += (Int64)(i & 0x7f) << sf;
                if ((i & 0x80) == 0) { break; }
                }
            return r;
            }

        internal static UInt64 ReadVUInt64(this BinaryReader reader) {
            if (reader == null) { throw new ArgumentNullException(nameof(reader)); }
            var sf = 0;
            var r  = 0UL;
            for (;(sf < 64);sf += 7) {
                var i = reader.ReadByte();
                r += (UInt64)(i & 0x7f) << sf;
                if ((i & 0x80) == 0) { break; }
                }
            return r;
            }

        internal static IDisposable StorePosition(this BinaryReader source)
            {
            return new PositionScope(source.BaseStream);
            }

        internal static IDisposable StorePosition(this Stream source)
            {
            return new PositionScope(source);
            }

        private class PositionScope : IDisposable
            {
            private Stream source;
            private readonly Int64 position;
            public PositionScope(Stream source)
                {
                this.source = source;
                position = source.Position;
                }

            public void Dispose()
                {
                source.Position = position;
                source = null;
                }
            }
        }
    }