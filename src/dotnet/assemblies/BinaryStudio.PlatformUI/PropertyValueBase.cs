using System;

namespace BinaryStudio.PlatformUI
    {
    public abstract class PropertyValueBase
        {
        public abstract UInt32 Format { get; }
        public Object Value { get; }
        public String TypeName { get { return typename ?? (typename = DeduceTypeName()); }}

        protected PropertyValueBase(Object value) {
            if (value == null) { throw new ArgumentNullException(nameof(value)); }
            Value = value;
            }

        protected PropertyValueBase(Object value, String type) {
            if (type == null) { throw new ArgumentNullException(nameof(type)); }
            Value = value;
            typename = type;
            }

        private String DeduceTypeName() {
            var r = TypeNameFromValue(Value);
            if (String.IsNullOrEmpty(r)) { throw new ArgumentException(); }
            return r;
            }

        protected abstract String TypeNameFromValue(Object value);
        private String typename;
        }
    }