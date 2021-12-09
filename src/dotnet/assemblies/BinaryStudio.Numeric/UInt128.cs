using System;

namespace BinaryStudio.Numeric
    {
    public struct UInt128
        {
        private unsafe fixed UInt32 value[4];

        /// <summary>
        /// Constructs <see cref="UInt128"/> structure from <see cref="UInt32"/> array by (high-to-low) ordering.
        /// </summary>
        /// <param name="source"></param>
        private unsafe UInt128(UInt32[] source)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Length != 4) { throw new ArgumentOutOfRangeException(nameof(source)); }
            value[0] = source[3];
            value[1] = source[2];
            value[2] = source[1];
            value[3] = source[0];
            }

        /// <summary>
        /// Constructs <see cref="UInt128"/> structure from <see cref="UInt64"/> array by (high-to-low) ordering.
        /// </summary>
        /// <param name="source"></param>
        private unsafe UInt128(UInt64[] source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Length != 2) { throw new ArgumentOutOfRangeException(nameof(source)); }
            fixed (void* target = value) {
                ((UInt64*)target)[0] = source[1];
                ((UInt64*)target)[1] = source[0];
                }
            }

        public unsafe UInt128(ref UInt64 hi, ref UInt64 lo) {
            fixed (void* target = value) {
                ((UInt64*)target)[0] = lo;
                ((UInt64*)target)[1] = hi;
                }
            }

        public unsafe UInt128(UInt64 hi, UInt64 lo) {
            fixed (void* target = value) {
                ((UInt64*)target)[0] = lo;
                ((UInt64*)target)[1] = hi;
                }
            }
        }
    }