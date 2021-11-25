using System;
using BinaryStudio.PlatformUI.Shell;

namespace BinaryStudio.PlatformUI
    {
    public class BuiltInPropertyValue : PropertyValueBase
        {
        private static readonly BuiltInPropertyValue FalseBooleanValue = new BuiltInPropertyValue(Boxes.BooleanFalse, "Boolean");
        private static readonly BuiltInPropertyValue TrueBooleanValue  = new BuiltInPropertyValue(Boxes.BooleanTrue,  "Boolean");
        private static readonly BuiltInPropertyValue ZeroInt32Value = new BuiltInPropertyValue(Boxes.Int32Zero,   "Int32");
        private static readonly BuiltInPropertyValue OneInt32Value  = new BuiltInPropertyValue(Boxes.Int32One,    "Int32");
        private static readonly BuiltInPropertyValue ZeroUInt32Value = new BuiltInPropertyValue(Boxes.UInt32Zero, "UInt32");
        private static readonly BuiltInPropertyValue OneUInt32Value  = new BuiltInPropertyValue(Boxes.UInt32One,  "UInt32");
        private static readonly BuiltInPropertyValue ZeroUInt64Value = new BuiltInPropertyValue(Boxes.UInt64Zero, "UInt64");
        private static readonly BuiltInPropertyValue ZeroDoubleValue = new BuiltInPropertyValue(Boxes.DoubleZero, "Double");
        private static readonly BuiltInPropertyValue EmptyStringValue = new BuiltInPropertyValue(String.Empty, "String");

        private BuiltInPropertyValue(String type)
            :base(null, type)
            {
            }

        private BuiltInPropertyValue(Object value)
            :base(value)
            {
            }

        private BuiltInPropertyValue(Object value, String type)
            :base(value, type)
            {
            }

        public override UInt32 Format { get { return 0; }}
        protected override String TypeNameFromValue(Object value)
            {
            throw new NotImplementedException();
            }

        public static BuiltInPropertyValue Create(Object value) {
            if (value is Boolean) { return BuiltInPropertyValue.Create((Boolean)value); }
            if (value is String)  { return BuiltInPropertyValue.Create((String)value);  }
            if (value is Int32)   { return BuiltInPropertyValue.Create((Int32)value);   }
            if (value is UInt32)  { return BuiltInPropertyValue.Create((UInt32)value);  }
            if (value is UInt64)  { return BuiltInPropertyValue.Create((UInt64)value);  }
            if (value is Double)  { return BuiltInPropertyValue.Create((Double)value);  }
            return new BuiltInPropertyValue(value);
            }

        public static BuiltInPropertyValue Create(Int32 value) {
            if (value == 0) { return BuiltInPropertyValue.ZeroInt32Value; }
            if (value != 1) { return new BuiltInPropertyValue(value, "Int32"); }
            return BuiltInPropertyValue.OneInt32Value;
            }

        public static BuiltInPropertyValue Create(UInt32 value) {
            if (value == 0u) { return BuiltInPropertyValue.ZeroUInt32Value; }
            if (value != 1u) { return new BuiltInPropertyValue(value, "UInt32"); }
            return BuiltInPropertyValue.OneUInt32Value;
            }

        public static BuiltInPropertyValue Create(UInt64 value) {
            if (value != 0uL) { return new BuiltInPropertyValue(value, "UInt64"); }
            return BuiltInPropertyValue.ZeroUInt64Value;
            }

        public static BuiltInPropertyValue Create(Double value) {
            return (value != 0.0)
                ? new BuiltInPropertyValue(value, "Double")
                : BuiltInPropertyValue.ZeroDoubleValue;
            }

        public static BuiltInPropertyValue Create(String value) {
            Validate.IsNotNull(value, "value");
            return (value.Length == 0)
                ? BuiltInPropertyValue.EmptyStringValue
                : new BuiltInPropertyValue(value, "String");
            }

        public static BuiltInPropertyValue Create(Boolean value) {
            return (value)
                ? TrueBooleanValue
                : FalseBooleanValue;
            }
        }
    }