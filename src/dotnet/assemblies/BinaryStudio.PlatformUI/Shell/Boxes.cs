using System;

namespace BinaryStudio.PlatformUI
    {
    public static class Boxes
        {
        public static readonly Object BooleanTrue = true;
        public static readonly Object BooleanFalse = false;
        public static readonly Object Int32Zero = 0;
        public static readonly Object Int32One = 1;
        public static readonly Object UInt32Zero = 0U;
        public static readonly Object UInt32One = 1U;
        public static readonly Object UInt64Zero = 0UL;
        public static readonly Object DoubleZero = 0.0;

        public static Object Box(Boolean value)
            {
            if (!value)
                return BooleanFalse;
            return BooleanTrue;
            }

        public static Object Box(Boolean? nullableValue)
            {
            if (!nullableValue.HasValue)
                return null;
            return Box(nullableValue.Value);
            }

        public static Object Box(Int32 value)
            {
            if (value == 0)
                return Int32Zero;
            if (value == 1)
                return Int32One;
            return value;
            }

        public static Object Box(UInt32 value)
            {
            if ((Int32)value == 0)
                return UInt32Zero;
            if ((Int32)value == 1)
                return UInt32One;
            return value;
            }

        public static Object Box(UInt64 value)
            {
            if ((Int64)value != 0L)
                return value;
            return UInt64Zero;
            }

        public static Object Box(Double value)
            {
            if (value != 0.0)
                return value;
            return DoubleZero;
            }
        }
    }