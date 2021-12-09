using System;

namespace BinaryStudio.Numeric
    {
    public struct UInt192
        {
        private unsafe fixed UInt32 value[6];

        /// <summary>
        /// Constructs <see cref="UInt192"/> structure from <see cref="UInt32"/> array by (high-to-low) ordering.
        /// </summary>
        /// <param name="source"></param>
        private unsafe UInt192(UInt32[] source)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Length != 6) { throw new ArgumentOutOfRangeException(nameof(source)); }
            value[0] = source[5];
            value[1] = source[4];
            value[2] = source[3];
            value[3] = source[2];
            value[4] = source[1];
            value[5] = source[0];
            }

        /// <summary>
        /// Constructs <see cref="UInt192"/> structure from <see cref="UInt64"/> array by (high-to-low) ordering.
        /// </summary>
        /// <param name="source"></param>
        private unsafe UInt192(UInt64[] source) {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Length != 3) { throw new ArgumentOutOfRangeException(nameof(source)); }
            fixed (void* target = value) {
                ((UInt64*)target)[0] = source[2];
                ((UInt64*)target)[1] = source[1];
                ((UInt64*)target)[2] = source[0];
                }
            }
        }
    }