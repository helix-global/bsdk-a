using System;

namespace BinaryStudio.Numeric
    {
    public struct UInt384
        {
        private unsafe fixed UInt32 value[12];

        /// <summary>
        /// Constructs <see cref="UInt384"/> structure from <see cref="UInt32"/> array by (high-to-low) ordering.
        /// </summary>
        /// <param name="source"></param>
        private unsafe UInt384(UInt32[] source)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Length != 12) { throw new ArgumentOutOfRangeException(nameof(source)); }
            value[ 0] = source[11];
            value[ 1] = source[10];
            value[ 2] = source[ 9];
            value[ 3] = source[ 8];
            value[ 4] = source[ 7];
            value[ 5] = source[ 6];
            value[ 6] = source[ 5];
            value[ 7] = source[ 4];
            value[ 8] = source[ 3];
            value[ 9] = source[ 2];
            value[10] = source[ 1];
            value[11] = source[ 0];
            }

        /// <summary>
        /// Constructs <see cref="UInt384"/> structure from <see cref="UInt64"/> array by (high-to-low) ordering.
        /// </summary>
        /// <param name="source"></param>
        private unsafe UInt384(UInt64[] source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Length != 6) { throw new ArgumentOutOfRangeException(nameof(source)); }
            fixed (void* target = value) {
                ((UInt64*)target)[0] = source[5];
                ((UInt64*)target)[1] = source[4];
                ((UInt64*)target)[2] = source[3];
                ((UInt64*)target)[3] = source[2];
                ((UInt64*)target)[4] = source[1];
                ((UInt64*)target)[5] = source[0];
                }
            }

        /// <summary>
        /// Constructs <see cref="UInt384"/> structure from <see cref="UInt128"/> array by (high-to-low) ordering.
        /// </summary>
        /// <param name="source"></param>
        private unsafe UInt384(UInt128[] source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Length != 3) { throw new ArgumentOutOfRangeException(nameof(source)); }
            fixed (void* target = value) {
                ((UInt128*)target)[0] = source[2];
                ((UInt128*)target)[1] = source[1];
                ((UInt128*)target)[2] = source[0];
                }
            }

        /// <summary>
        /// Constructs <see cref="UInt384"/> structure from <see cref="UInt192"/> array by (high-to-low) ordering.
        /// </summary>
        /// <param name="source"></param>
        private unsafe UInt384(UInt192[] source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Length != 2) { throw new ArgumentOutOfRangeException(nameof(source)); }
            fixed (void* target = value) {
                ((UInt192*)target)[0] = source[1];
                ((UInt192*)target)[1] = source[0];
                }
            }

        private unsafe UInt384(ref UInt192 hi, ref UInt192 lo) {
            fixed (void* target = value) {
                ((UInt192*)target)[0] = lo;
                ((UInt192*)target)[1] = hi;
                }
            }
        }
    }