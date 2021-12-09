using System;

namespace BinaryStudio.Numeric
    {
    public struct UInt224
        {
        private unsafe fixed UInt32 value[7];

        /// <summary>
        /// Constructs <see cref="UInt224"/> structure from <see cref="UInt32"/> array by (high-to-low) ordering.
        /// </summary>
        /// <param name="source"></param>
        private unsafe UInt224(UInt32[] source)
            {
            if (source == null) { throw new ArgumentNullException(nameof(source)); }
            if (source.Length != 7) { throw new ArgumentOutOfRangeException(nameof(source)); }
            value[0] = source[6];
            value[1] = source[5];
            value[2] = source[4];
            value[3] = source[3];
            value[4] = source[2];
            value[5] = source[1];
            value[6] = source[0];
            }
        }
    }