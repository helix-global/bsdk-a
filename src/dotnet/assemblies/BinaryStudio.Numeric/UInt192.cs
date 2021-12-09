using System;

// ReSharper disable once LocalVariableHidesMember

namespace BinaryStudio.Numeric
    {
    public struct UInt192 : IComparable<UInt192>, IComparable
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

        public Int32 CompareTo(UInt192 other)
            {
            return CompareTo(ref other);
            }

        public unsafe Int32 CompareTo(ref UInt192 other)
            {
            fixed (void* x = value)
            fixed (void* y = other.value) {
                Int32 r;
                if ((r = (((UInt64*) x)[2]).CompareTo(((UInt64*) y)[2])) != 0) return r;
                if ((r = (((UInt64*) x)[1]).CompareTo(((UInt64*) y)[1])) != 0) return r;
                return (((UInt64*)x)[0]).CompareTo(((UInt64*)y)[0]);
                }
            }

        public Int32 CompareTo(Object other) {
            if (other == null) { return +1; }
            if (other is UInt192 value) {
                return CompareTo(ref value);
                }
            throw new ArgumentOutOfRangeException(nameof(other));
            }
        }
    }