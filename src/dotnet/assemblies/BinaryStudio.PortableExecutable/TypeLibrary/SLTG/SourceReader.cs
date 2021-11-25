using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace BinaryStudio.PortableExecutable.TypeLibrary
    {
    internal class SourceReader : BinaryReader
        {
        public SourceReader(Stream input)
            : base(input, new UTF8Encoding())
            {
            }

        public SourceReader(Stream input, Encoding encoding)
            : base(input, encoding)
            {
            }

        /// <summary>Reads a string from the current stream. The string is prefixed with the length, encoded as an short integer.</summary>
        /// <returns>The string being read.</returns>
        /// <exception cref="EndOfStreamException">The end of the stream is reached. </exception>
        /// <exception cref="ObjectDisposedException">The stream is closed. </exception>
        /// <exception cref="IOException">An I/O error occurs. </exception>
        /// <filterpriority>2</filterpriority>
        public override String ReadString()
            {
            var c = ReadInt16();
            if (c == -1) { return null; }
            var r = new Byte[c];
            var n = Read(r, 0, c);
            if (n != c) { throw new EndOfStreamException(); }
            return Encoding.ASCII.GetString(r);
            }

        public unsafe Guid ReadGuid() {
            fixed (Byte* r = ReadBytes(sizeof(Guid))) {
                return *((Guid*)r);
                }
            }

        public unsafe void ReadBlock<T>(out T target)
        where T: struct
            {
            fixed (Byte* r = ReadBytes(Marshal.SizeOf(typeof(T)))) {
                target = (T)Marshal.PtrToStructure((IntPtr)r, typeof(T));
                }
            }

        public override String ToString()
            {
            if (BaseStream is SourceStream) { return BaseStream.ToString(); }
            var i = ((Single)BaseStream.Position/BaseStream.Length)*100.0;
            return $"{i:F2}%";
            }
        }
    }