using System;

namespace BinaryStudio.Numeric
    {
    public struct UInt192
        {
        private unsafe fixed UInt32 value[6];
        private unsafe UInt192(UInt32[] source)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Length != 6) { throw new ArgumentOutOfRangeException(nameof(source)); }
            value[0] = source[0];
            value[1] = source[1];
            value[2] = source[2];
            value[3] = source[3];
            value[4] = source[4];
            value[5] = source[5];
            }

        private unsafe UInt192(UInt64[] source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Length != 3) { throw new ArgumentOutOfRangeException(nameof(source)); }
            fixed (void* target = value) {
                ((UInt64*)target)[0] = source[0];
                ((UInt64*)target)[1] = source[1];
                ((UInt64*)target)[2] = source[2];
                }
            }
        }
    }