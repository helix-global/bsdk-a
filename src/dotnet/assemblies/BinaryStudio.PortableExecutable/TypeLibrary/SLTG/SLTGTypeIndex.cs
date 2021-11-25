using System;
using System.Globalization;

namespace BinaryStudio.PortableExecutable.TypeLibrary
    {
    internal class SLTGTypeIndex
        {
        public int Index { get; }
        public Int32 GlobalIndex { get; }
        public String Source { get; }
        public SLTGTypeIndex(String source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            Source = source;
            Index = -1;
            GlobalIndex = -1;
            var i = source.IndexOf("#");
            if (i != -1) {
                Index = Int32.Parse(source.Substring(i + 1), NumberStyles.HexNumber);
                }
            if (source.StartsWith("*\\R")) {
                var n = Int32.Parse(source.Substring(3, 4), NumberStyles.HexNumber);
                if ((n & 0x8000) == 0x8000) {
                    n = n | -1;
                    }
                GlobalIndex = n;
                }
            }

        public override String ToString()
            {
            return Source;
            }
        }
    }