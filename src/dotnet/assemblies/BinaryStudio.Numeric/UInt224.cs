using System;

namespace BinaryStudio.Numeric
    {
    public struct UInt224
        {
        private unsafe fixed UInt32 value[7];
        private unsafe UInt224(UInt32[] source)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Length != 7) { throw new ArgumentOutOfRangeException(nameof(source)); }
            value[0] = source[0];
            value[1] = source[1];
            value[2] = source[2];
            value[3] = source[3];
            value[4] = source[4];
            value[5] = source[5];
            value[6] = source[6];
            }
        }
    }