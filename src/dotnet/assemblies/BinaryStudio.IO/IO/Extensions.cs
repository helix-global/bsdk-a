using System;
using System.IO;

namespace BinaryStudio.IO
    {
    public static class Extensions
        {
        #if NET35
        public static void CopyTo(this Stream source, Stream target)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (target == null) { throw new ArgumentNullException(nameof(target)); }
            var buffer = new Byte[8192];
            while (true) {
                var size = source.Read(buffer, 0, 8192);
                if (size == 0) { break; }
                target.Write(buffer, 0, size);
                }
            }
        #endif

        public static void CopyTo(this Stream source, Stream target, Int32 buffersize, Int64 length)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (target == null) { throw new ArgumentNullException(nameof(target)); }
            if (buffersize < 1) { throw new ArgumentOutOfRangeException(nameof(buffersize)); }
            if (length < 1)     { throw new ArgumentOutOfRangeException(nameof(length));     }
            var buffer = new Byte[buffersize];
            while (length > 0) {
                var size = source.Read(buffer, 0, (Int32)Math.Min((Int64)buffersize,length));
                if (size == 0) { break; }
                target.Write(buffer, 0, size);
                length -= size;
                }
            }

        public static IDisposable StorePosition(this BinaryReader source)
            {
            return new PositionScope(source.BaseStream);
            }

        public static IDisposable StorePosition(this Stream source)
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
            }        }
    }