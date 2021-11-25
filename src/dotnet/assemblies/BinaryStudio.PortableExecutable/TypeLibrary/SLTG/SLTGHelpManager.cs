using System;
using System.Collections.Generic;
using System.Text;

namespace BinaryStudio.PortableExecutable.TypeLibrary
    {
    using UShort = UInt16;
    using ULong  = UInt32;
    using Long   = Int32;

    internal class SLTGHelpManager
        {
        private const Byte FIRST_BIT    = 0x80;
        private const Byte SECOND_BIT   = 0x40;
        private const Byte THIRD_BIT    = 0x20;
        private const Byte FOURTH_BIT   = 0x10;
        private const Byte FIFTH_BIT    = 0x08;
        private const Byte SIXTH_BIT    = 0x04;
        private const Byte SEVENTH_BIT  = 0x02;
        private const Byte EIGHTH_BIT   = 0x01;

        private class Node
            {
            public String Text;
            public Node Left;
            public Node Right;

            public override String ToString()
                {
                var r = new List<String>();
                if (Text != null)  { r.Add("Text");  }
                if (Left != null)  { r.Add("Left");  }
                if (Right != null) { r.Add("Right"); }
                return String.Join(",", r);
                }
            }

        private Node Source { get; }
        public UShort MaxStringSize { get; }
        public Long Size { get; }

        public unsafe SLTGHelpManager(SourceReader reader) {
            if (reader == null) { throw new ArgumentNullException(nameof(reader)); }
            MaxStringSize = reader.ReadUInt16();
            var huffmantreesz = reader.ReadInt32();
            if (huffmantreesz != 0) {
                fixed (Byte* source = reader.ReadBytes(huffmantreesz)) {
                    Source = Build(new Node(), source, source);
                    }
                }
            Size = huffmantreesz;
            }

        #region M:Build(Node,Byte*,Byte*):Node
        private static unsafe Node Build(Node target, Byte* source, Byte* @base) {
            if ((*source & FIRST_BIT) != FIRST_BIT) {
                ++source;
                var r = new List<Byte>();
                while (*source != 0) {
                    r.Add(*source);
                    ++source;
                    }
                target.Text = Encoding.ASCII.GetString(r.ToArray());
                }
            else
                {
                target.Left = Build(new Node(), source + 3, @base);
                var offset =
                    (((ULong)(source[0] & 0x7f)) << 16) |
                    (((ULong)(source[1])) << 8) |
                    (((ULong)(source[2])));
                target.Right = Build(new Node(), @base + offset, @base);
                }
            return target;
            }
        #endregion
        #region M:Decode(Byte[]):String
        public unsafe String Decode(Byte[] source) {
            if ((source == null) || (source.Length == 0)) { return null; }
            fixed (Byte* src = source) {
                var n = 0;
                var r = new List<String>();
                for (;;) {
                    var value = Decode(Source, src, ref n);
                    if (String.IsNullOrEmpty(value)) { break; }
                    r.Add(value);
                    }
                return String.Join(" ", r);
                }
            }
        #endregion
        #region M:Decode(Byte*):String
        public unsafe String Decode(Byte* source) {
            if ((source == null)) { return null; }
            var n = 0;
            var r = new List<String>();
            for (;;) {
                var value = Decode(Source, source, ref n);
                if (String.IsNullOrEmpty(value)) { break; }
                r.Add(value);
                }
            return String.Join(" ", r);
            }
        #endregion
        #region M:Decode(Node,Byte*,ref Int32):String
        private static unsafe String Decode(Node node, Byte* source, ref Int32 n) {
            if (node.Text != null) { return node.Text; }
            var r = IsLeft(n, source);
            n++;
            return r
                ? Decode(node.Left,  source, ref n)
                : Decode(node.Right, source, ref n);
            }
        #endregion
        #region M:IsLeft(Int32,Byte*):Boolean
        private static unsafe Boolean IsLeft(Int32 n, Byte* source) {
            var i = n/8;
            switch ((n % 8) + 1) {
                case 1: { return (source[i] & FIRST_BIT)   == FIRST_BIT;   }
                case 2: { return (source[i] & SECOND_BIT)  == SECOND_BIT;  }
                case 3: { return (source[i] & THIRD_BIT)   == THIRD_BIT;   }
                case 4: { return (source[i] & FOURTH_BIT)  == FOURTH_BIT;  }
                case 5: { return (source[i] & FIFTH_BIT)   == FIFTH_BIT;   }
                case 6: { return (source[i] & SIXTH_BIT)   == SIXTH_BIT;   }
                case 7: { return (source[i] & SEVENTH_BIT) == SEVENTH_BIT; }
                case 8: { return (source[i] & EIGHTH_BIT)  == EIGHTH_BIT;  }
                default: throw new ArgumentOutOfRangeException(nameof(n));
                }
            }
        #endregion
        }
    }