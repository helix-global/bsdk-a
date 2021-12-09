using System;

namespace BinaryStudio.Numeric
    {
    public struct UInt128
        {
        private unsafe fixed UInt32 value[4];
        private unsafe UInt128(UInt32[] source)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Length != 4) { throw new ArgumentOutOfRangeException(nameof(source)); }
            value[0] = source[0];
            value[1] = source[1];
            value[2] = source[2];
            value[3] = source[3];
            }

        private unsafe UInt128(UInt64[] source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Length != 2) { throw new ArgumentOutOfRangeException(nameof(source)); }
            fixed (void* target = value) {
                ((UInt64*)target)[0] = source[0];
                ((UInt64*)target)[1] = source[1];
                }
            }

        private unsafe UInt128(ref UInt64 x, ref UInt64 y) {
            fixed (void* target = value) {
                ((UInt64*)target)[0] = x;
                ((UInt64*)target)[1] = y;
                }
            }
        }
    }