using System;

namespace BinaryStudio.Numeric
    {
    public struct UInt512
        {
        private unsafe fixed UInt32 value[16];

        /// <summary>
        /// Constructs <see cref="UInt512"/> structure from <see cref="UInt32"/> array by (high-to-low) ordering.
        /// </summary>
        /// <param name="source"></param>
        private unsafe UInt512(UInt32[] source)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Length != 16) { throw new ArgumentOutOfRangeException(nameof(source)); }
            value[ 0] = source[15];
            value[ 1] = source[14];
            value[ 2] = source[13];
            value[ 3] = source[12];
            value[ 4] = source[11];
            value[ 5] = source[10];
            value[ 6] = source[ 9];
            value[ 7] = source[ 8];
            value[ 8] = source[ 7];
            value[ 9] = source[ 6];
            value[10] = source[ 5];
            value[11] = source[ 4];
            value[12] = source[ 3];
            value[13] = source[ 2];
            value[14] = source[ 1];
            value[15] = source[ 0];
            }

        /// <summary>
        /// Constructs <see cref="UInt512"/> structure from <see cref="UInt64"/> array by (high-to-low) ordering.
        /// </summary>
        /// <param name="source"></param>
        private unsafe UInt512(UInt64[] source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Length != 6) { throw new ArgumentOutOfRangeException(nameof(source)); }
            fixed (void* target = value) {
                ((UInt64*)target)[0] = source[7];
                ((UInt64*)target)[1] = source[6];
                ((UInt64*)target)[2] = source[5];
                ((UInt64*)target)[3] = source[4];
                ((UInt64*)target)[4] = source[3];
                ((UInt64*)target)[5] = source[2];
                ((UInt64*)target)[6] = source[1];
                ((UInt64*)target)[7] = source[0];
                }
            }

        /// <summary>
        /// Constructs <see cref="UInt512"/> structure from <see cref="UInt128"/> array by (high-to-low) ordering.
        /// </summary>
        /// <param name="source"></param>
        private unsafe UInt512(UInt128[] source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Length != 4) { throw new ArgumentOutOfRangeException(nameof(source)); }
            fixed (void* target = value) {
                ((UInt128*)target)[0] = source[3];
                ((UInt128*)target)[1] = source[2];
                ((UInt128*)target)[2] = source[1];
                ((UInt128*)target)[3] = source[0];
                }
            }

        /// <summary>
        /// Constructs <see cref="UInt512"/> structure from <see cref="UInt256"/> array by (high-to-low) ordering.
        /// </summary>
        /// <param name="source"></param>
        private unsafe UInt512(UInt256[] source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Length != 2) { throw new ArgumentOutOfRangeException(nameof(source)); }
            fixed (void* target = value) {
                ((UInt256*)target)[0] = source[1];
                ((UInt256*)target)[1] = source[0];
                }
            }

        private unsafe UInt512(ref UInt256 hi, ref UInt256 lo) {
            fixed (void* target = value) {
                ((UInt256*)target)[0] = lo;
                ((UInt256*)target)[1] = hi;
                }
            }
        }
    }