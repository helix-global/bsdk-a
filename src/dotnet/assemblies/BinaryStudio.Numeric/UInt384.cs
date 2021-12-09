using System;

namespace BinaryStudio.Numeric
    {
    public struct UInt384
        {
        private unsafe fixed UInt32 value[12];   
        private unsafe UInt384(UInt32[] source)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Length != 12) { throw new ArgumentOutOfRangeException(nameof(source)); }
            value[ 0] = source[ 0];
            value[ 1] = source[ 1];
            value[ 2] = source[ 2];
            value[ 3] = source[ 3];
            value[ 4] = source[ 4];
            value[ 5] = source[ 5];
            value[ 6] = source[ 6];
            value[ 7] = source[ 7];
            value[ 8] = source[ 8];
            value[ 9] = source[ 9];
            value[10] = source[10];
            value[11] = source[11];
            }

        private unsafe UInt384(UInt64[] source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Length != 6) { throw new ArgumentOutOfRangeException(nameof(source)); }
            fixed (void* target = value) {
                ((UInt64*)target)[0] = source[0];
                ((UInt64*)target)[1] = source[1];
                ((UInt64*)target)[2] = source[2];
                ((UInt64*)target)[3] = source[3];
                ((UInt64*)target)[4] = source[4];
                ((UInt64*)target)[5] = source[5];
                }
            }

        private unsafe UInt384(UInt128[] source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Length != 3) { throw new ArgumentOutOfRangeException(nameof(source)); }
            fixed (void* target = value) {
                ((UInt128*)target)[0] = source[0];
                ((UInt128*)target)[1] = source[1];
                ((UInt128*)target)[2] = source[2];
                }
            }

        private unsafe UInt384(UInt192[] source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Length != 2) { throw new ArgumentOutOfRangeException(nameof(source)); }
            fixed (void* target = value) {
                ((UInt192*)target)[0] = source[0];
                ((UInt192*)target)[1] = source[1];
                }
            }

        private unsafe UInt384(ref UInt192 x, ref UInt192 y) {
            fixed (void* target = value) {
                ((UInt192*)target)[0] = x;
                ((UInt192*)target)[1] = y;
                }
            }
        }
    }