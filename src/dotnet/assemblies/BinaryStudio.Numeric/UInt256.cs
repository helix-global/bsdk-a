using System;

namespace BinaryStudio.Numeric
    {
    public struct UInt256
        {
        private unsafe fixed UInt32 value[8];
        private unsafe UInt256(UInt32[] source)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Length != 8) { throw new ArgumentOutOfRangeException(nameof(source)); }
            value[0] = source[0];
            value[1] = source[1];
            value[2] = source[2];
            value[3] = source[3];
            value[4] = source[4];
            value[5] = source[5];
            value[6] = source[6];
            value[7] = source[7];
            }

        private unsafe UInt256(UInt64[] source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Length != 4) { throw new ArgumentOutOfRangeException(nameof(source)); }
            fixed (void* target = value) {
                ((UInt64*)target)[0] = source[0];
                ((UInt64*)target)[1] = source[1];
                ((UInt64*)target)[2] = source[2];
                ((UInt64*)target)[3] = source[3];
                }
            }

        private unsafe UInt256(UInt128[] source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Length != 2) { throw new ArgumentOutOfRangeException(nameof(source)); }
            fixed (void* target = value) {
                ((UInt128*)target)[0] = source[0];
                ((UInt128*)target)[1] = source[1];
                }
            }

        private unsafe UInt256(ref UInt128 x, ref UInt128 y) {
            fixed (void* target = value) {
                ((UInt128*)target)[0] = x;
                ((UInt128*)target)[1] = y;
                }
            }
        }
    }